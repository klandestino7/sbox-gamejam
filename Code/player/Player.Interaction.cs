namespace Gamejam;

public partial class Player
{
	private const float INTERACTION_DISTANCE = 100f;
	private const float INTERACTION_SIZE = 10f;
	[Sync] public TimedAction TimedAction { get; private set; }
	public bool HasTimedAction => TimedAction is not null;

	public Vector2 Cursor { get; set; }
	public Ray ViewRay => new( Camera.Transform.Position, Camera.Transform.Rotation.Forward );
	public GameObject TargetedGameObject { get; private set; }
	public SceneTraceResult InteractionTrace { get; private set; }
	public BBox? InteractionBounds { get; private set; }
	public int ContextActionId { get; private set; }

	private List<GameObject> LastEntitiesInRange { get; set; } = new();
	
	public void ResetCursor()
	{
		Cursor = new Vector2( 0.5f, 0.5f );
	}

	public void SetContextAction( ContextAction action )
	{
		ContextActionId = action.Hash;
	}
	private void UpdateInteractions()
	{
		var thinTrace = Scene.Trace.Ray( ViewRay, INTERACTION_DISTANCE )
					.IgnoreGameObject( GameObject )
					.WithoutTags( "world" )
					.Run();

		// var	trace = Scene.Trace.Ray( Transform.Position, Camera.Transform.Position * Vector3.Forward )
		// 			.Size( 4f )
		// 			.WithoutTags( "world" )
		// 			.Run();

		// Log.Info($"trace :: {thinTrace.GameObject}");


		TargetedGameObject = thinTrace.GameObject;

		var actions = TargetedGameObject as IContextActionProvider;
		var actionId = ContextActionId;

		if ( !IsProxy )
		{
			var entities = Scene.Trace.Sphere( 500f, new Ray( Transform.Position, Vector3.Forward ), 0f )
							.WithTag( "interaction" )
							.IgnoreGameObject( GameObject )
							.RunAll();

			foreach ( var entity in LastEntitiesInRange )
			{
				var glow = entity.Components.Get<HighlightOutline>();
				glow.Enabled = false;
			}

			LastEntitiesInRange.Clear();

			foreach ( var entity in entities )
			{
				// if ( entity.GameObject is not IContextActionProvider)
				// 		return;

				var contextAction = entity.Component.Components.Get<IContextActionProvider>();
				// Log.Info($" contextAction :: {contextAction}");

				if ( Transform.Position.Distance( entity.GameObject.Transform.Position ) > contextAction.InteractionRange * 3f )
					continue;

				if ( !contextAction.AlwaysGlow )
					continue;

				var glow = entity.Component.Components.GetOrCreate<HighlightOutline>();
				glow.InsideObscuredColor = contextAction.GlowColor.WithAlpha( 1f );
				glow.Color = contextAction.GlowColor.WithAlpha( 1f );
				glow.Width = 1f;
				glow.Enabled = true;

				LastEntitiesInRange.Add( entity.GameObject );
			}

			// if ( actions.IsValid() && Transform.Position.Distance( actions.Position ) <= actions.InteractionRange )
			// {
			// 	var glow = TargetedGameObject.Components.GetOrCreate<HighlightOutline>();
			// 	glow.InsideObscuredColor = actions.GlowColor.WithAlpha( 0.8f );
			// 	glow.Color = actions.GlowColor;
			// 	glow.Width = 0.2f;
			// 	glow.Enabled = true;

			// 	LastEntitiesInRange.Add( actions );
			// }

			ContextActionId = 0;
		}

		if ( actions.IsValid() && Transform.Position.Distance( actions.Position ) <= actions.InteractionRange )
		{
			if ( actionId != 0 )
			{
				var allActions = IContextActionProvider.GetAllActions( this, actions );
				var action = allActions.Where( a => a.IsAvailable( this ) && a.Hash == actionId ).FirstOrDefault();

				if ( action.IsValid() )
				{
					actions.OnContextAction( this, action );
					// return true;
				}
			}
		}

		// return false;
	}

}
