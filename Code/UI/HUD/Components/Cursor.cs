using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gamejam.UI;

public class CursorAction : Panel
{
	public ContextAction Action { get; private set; }

	public Player Player => PlayerState.Viewer.Player;
	private Image Icon { get; set; }
	private Label Name { get; set; }
	private Label Condition { get; set; }

	public CursorAction()
	{
		Icon = Add.Image( "", "icon" );
		Name = Add.Label( "", "name" );
		Condition = Add.Label( "", "condition" );

		BindClass( "visible", () => Action.IsValid() );
	}

	public bool Select()
	{
		if ( Action.IsValid() && Action.IsAvailable( Player ) )
		{
			Player.SetContextAction( Action );
			
			return true;
		}

		return false;
	}

	public void ClearAction()
	{
		Action = null;
	}

	public void SetAction( ContextAction action )
	{
		if ( !string.IsNullOrEmpty( action.Icon ) )
		{
			Icon.Texture = Texture.Load( FileSystem.Mounted, action.Icon );
		}

		Name.Text = action.Name;
		Action = action;

		UpdateAvailability();
	}

	public override void Tick()
	{
		UpdateAvailability();
		base.Tick();
	}

	private void UpdateAvailability()
	{
		if ( Action.IsValid() )
		{
			var availability = Action.GetAvailability( Player.Local );
			Condition.Text = availability.Message;
			SetClass( "unavailable", !availability.IsAvailable );
		}
	}
}

[StyleSheet( "/UI/HUD/Components/Cursor.scss" )]
public class Cursor : Panel
{
	public static Cursor Current { get; private set; }

	private IContextActionProvider ActionProvider { get; set; }
	private CursorAction PrimaryAction { get; set; }
	private TimeSince TimeSincePressed { get; set; }
	private bool DisableSecondaryActions { get; set; }
	private Panel ActionContainer { get; set; }
	private bool IsSecondaryOpen { get; set; }
	private Vector2 ActionCursorPosition { get; set; }
	private TimeSince LastActionTime { get; set; }
	private int ActionHash { get; set; }
	private Panel PlusMoreIcon { get; set; }
	private Panel ActionCursor { get; set; }
	private Label Title { get; set; }

	public bool HasMoreOptions => ActionContainer.ChildrenCount > 0;

	public Player Player => PlayerState.Viewer.Player;

	public GameObject TargetedGameObject => Player.TargetedGameObject;

	public Cursor()
	{
		LastActionTime = 0f;
		PrimaryAction = AddChild<CursorAction>( "primary-action" );
		PlusMoreIcon = Add.Panel( "plus-more" );
		ActionContainer = Add.Panel( "actions" );
		Title = Add.Label( "", "title" );
		ActionCursor = Add.Panel( "action-cursor" );
		Current = this;
	}

	public override void Tick()
	{
		if ( !Player.IsValid() ) {			
			return;
		}

		OnUpdateFixed();

		if ( TargetedGameObject == null || !TargetedGameObject.IsValid() ) {
			ClearActionProvider();
			return;
		}

		var provider = TargetedGameObject.Components.Get<IContextActionProvider>();

		if ( provider == null ) return;

		if ( Player.HasTimedAction )
		{
			LastActionTime = 0f;
		}

		SetClass( "recent-action", LastActionTime < 0.5f );
		// SetClass( "is-aiming", player.IsAiming() );

		if ( LastActionTime > 0.5f && TargetedGameObject.IsValid() && Player.Transform.Position.Distance( TargetedGameObject.Transform.Position ) <= provider.InteractionRange )
			SetActionProvider( provider );
		else
			ClearActionProvider();
	}

	private int GetActionHash( ContextAction primary, IEnumerable<ContextAction> secondaries )
	{
		var hash = 0;

		if ( primary.IsValid() )
		{
			var availability = primary.GetAvailability( Player.Local );
			hash = HashCode.Combine( hash, primary, availability.IsAvailable );
		}

		foreach ( var action in secondaries )
		{
			var availability = primary.GetAvailability( Player.Local );
			hash = HashCode.Combine( hash, action, availability.IsAvailable );
		}

		return hash;
	}

	private void SetActionProvider( IContextActionProvider provider )
	{
		var primary = provider.GetPrimaryAction( Player.Local );
		var secondaries = provider.GetSecondaryActions( Player.Local );
		var hash = GetActionHash( primary, secondaries );

		if ( ActionProvider == provider && ActionHash == hash )
		{
			return;
		}

		ActionProvider = provider;
		ActionHash = hash;

		if ( !primary.IsValid() )
		{
			primary = secondaries.FirstOrDefault( s => s.IsAvailable( Player.Local ) );

			if ( !primary.IsValid() )
			{
				ClearActionProvider();
				return;
			}	
		}

		ActionContainer.DeleteChildren( true );

		foreach ( var secondary in secondaries )
		{
			if ( secondary == primary )
				continue;

			var action = new CursorAction();
			action.SetAction( secondary );
			ActionContainer.AddChild( action );
		}

		PrimaryAction.SetAction( primary );

		Title.Text = provider.GetContextName();

		SetClass( "was-deleted", false );
		SetClass( "has-secondary", ActionContainer.ChildrenCount > 0 );
		SetClass( "has-actions", true );
	}

	private void ClearActionProvider()
	{
		if ( ActionProvider == null )
			return;

		PrimaryAction?.ClearAction();
		ActionContainer?.DeleteChildren( true );

		Title.Text = null;

		SetClass( "was-deleted", !ActionProvider.IsValid() );
		SetClass( "has-secondary", false );
		SetClass( "has-actions", false );

		ActionProvider = null;
	}

	public void OnUpdateFixed()
	{
		var hasSecondaries = ActionContainer.ChildrenCount > 0;
		var secondaryHoldDelay = 0.25f;

		if ( !ActionProvider.IsValid() || IsHidden() || LastActionTime < 0.5f )
		{
			DisableSecondaryActions = true;
			IsSecondaryOpen = false;
			return;
		}

		if ( Input.Pressed( InputAction.Use ) )
		{
			DisableSecondaryActions = false;
			TimeSincePressed = 0f;
			IsSecondaryOpen = false;
		}

		if ( !DisableSecondaryActions )
		{
			if ( Input.Down( InputAction.Use ) && hasSecondaries )
			{
				if ( TimeSincePressed > secondaryHoldDelay && !IsSecondaryOpen )
				{
					ActionCursorPosition = Vector2.Zero;
					IsSecondaryOpen = true;
				}
			}
		}

		if ( IsSecondaryOpen )
		{
			UpdateActionCursor();
			return;
		}

		if ( Input.Released( InputAction.Use ) && ( !hasSecondaries || TimeSincePressed < secondaryHoldDelay ) )
		{
			if ( PrimaryAction.Select() )
			{
				LastActionTime = 0f;
				return;
			}
		}
	}

	private void UpdateActionCursor()
	{
		var mouseDelta = Input.MouseDelta;

		ActionCursorPosition += (mouseDelta * 10f * Time.Delta);
		ActionCursorPosition = ActionCursorPosition.Clamp( Vector2.One * -500f, Vector2.One * 500f );

		CursorAction closestItem = null;
		var closestDistance = 0f;
		var globalPosition = Box.Rect.Center + ActionCursorPosition;

		var children = ActionContainer.ChildrenOfType<CursorAction>();

		foreach ( var child in children )
		{
			var distance = child.Box.Rect.Center.Distance( globalPosition );

			if ( distance <= 32f && (closestItem == null || distance < closestDistance ) )
			{
				closestDistance = distance;
				closestItem = child;
			}

			child.SetClass( "is-hovered", false );
		}

		ActionCursor.Style.Left = Length.Pixels( ActionCursorPosition.x * ScaleFromScreen );
		ActionCursor.Style.Top = Length.Pixels( ActionCursorPosition.y * ScaleFromScreen );

		if ( closestItem != null )
		{
			closestItem.SetClass( "is-hovered", true );

			if ( Input.Released( InputAction.Use ) )
			{
				if ( closestItem.Select() )
				{
					LastActionTime = 0f;
				}
			}
		}

		if ( !Input.Down( InputAction.Use ) )
		{
			DisableSecondaryActions = true;
			IsSecondaryOpen = false;
		}

		// Input.StopProcessing = true;
		Input.AnalogMove = Vector2.Zero;
		Input.AnalogLook = Angles.Zero;
	}

	private bool IsHidden()
	{
		var player = Player.Local;

		// if ( !player.IsValid() || player.LifeState == LifeState.Dead )
		// 	return true;

		// if ( ToolboxMenu.Current?.IsOpen ?? false )
		// 	return true;

		// if ( ReloadMenu.Current?.IsOpen ?? false )
		// 	return true;

		// if ( player.HasTimedAction )
		// 	return true;

		// if ( Dialog.IsActive() )
		// 	return true;

		return false;
	}
    protected override int BuildHash()
    {
        return !Player.IsValid() ? 0 : HashCode.Combine( Player.TargetedGameObject );
    }
	protected override void OnParametersSet()
	{
		BindClass( "secondary-open", () => IsSecondaryOpen );
		BindClass( "hidden", IsHidden );

		base.OnParametersSet();
	}
}
