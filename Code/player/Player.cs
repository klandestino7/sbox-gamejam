using Sandbox;
using Sandbox.Citizen;
using Sandbox.Diagnostics;

namespace Gamejam;

public partial class Player : Component, Component.ExecuteInEditor
{
	public CameraComponent ViewModelCamera;

	/// <summary>
	/// The position this player last spawned at.
	/// </summary>
	[HostSync]
	public Vector3 SpawnPosition { get; set; }

	/// <summary>
	/// The rotation this player last spawned at.
	/// </summary>
	[HostSync]
	public Rotation SpawnRotation { get; set; }

	/// <summary>
	/// The player state ID
	/// </summary>
	[HostSync] public PlayerState PlayerState { get; private set; }

	[Property] public GameObject EyePos;
	[Property] public SkinnedModelRenderer PlayerBody;
	[Property] public CharacterController CharacterController { get; set; }
	[Property] public CitizenAnimationHelper AnimationHelper { get; set; }
	[Property] public virtual HealthSystem HealthSystem { get; set; }
	[Property] public virtual SpotLight SpotLight { get; set; }
	protected BoxCollider Collider;

	public Inventory Inventory { get; private set; }

	protected override void OnStart()
	{
		ViewModelCamera = Scene.GetAllComponents<CameraComponent>().Where( x => x.IsMainCamera ).FirstOrDefault();
		PlayerBody = Components.Get<SkinnedModelRenderer>( FindMode.EverythingInSelfAndDescendants );
		// Collider = Components.Get<BoxCollider>( FindMode.EverythingInSelfAndDescendants );

		Inventory = Components.Get<Inventory>( FindMode.EverythingInSelfAndDescendants );;
		SpotLight = GameObject.Components.Get<SpotLight>( FindMode.InChildren );
	}

	protected override void OnUpdate()
	{
		if ( !IsProxy )
		{
			MouseInput();
			Transform.Rotation = new Angles( 0, EyeAngles.yaw, 0 );
		}

		UpdateAnimation();
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy )
			return;

		CrouchingInput();
		MovementInput();
		// UpdateAttack();

		UpdateStamina();
		RestoreStamina();
	}
	protected override void OnPreRender()
	{
		UpdateBodyVisibility();
		
		if ( IsProxy )
			return;

		UpdateCamera();
	}

	public void UpdateStamina() 
	{
		if ( Input.Down( "run" ) && !WishVelocity.IsNearlyZero() ) {
			HealthSystem.DrainStamina( 0.1f ); 
		}
	}

	private void RestoreStamina()
	{
		var staminaAmountIncrease = 0.1f;

		if ( !WishVelocity.IsNearlyZero() ) {
			staminaAmountIncrease = 0.02f;
		}
		
		if (HealthSystem.Stamina < HealthSystem.MaxStamina)
		{
			HealthSystem.Stamina = Math.Clamp( HealthSystem.Stamina + staminaAmountIncrease, 0, HealthSystem.MaxStamina );
		}
	}
	
	private void UpdateBodyVisibility()
	{
		if ( AnimationHelper is null )
			return;

		var renderMode = ModelRenderer.ShadowRenderType.On;
		if ( !IsProxy ) renderMode = ModelRenderer.ShadowRenderType.ShadowsOnly;

		AnimationHelper.Target.RenderType = renderMode;

		var playerModels = GameObject.Components.GetAll<ModelRenderer>( FindMode.InChildren );

		foreach ( var clothing in playerModels )
		{
			if ( !clothing.Tags.Has( "clothing" ) )
				continue;

			clothing.RenderType = renderMode;
		}
	}
}
