using Sandbox;
using Sandbox.Citizen;
using Sandbox.Diagnostics;

namespace Gamejam;

public partial class Player : Component, Component.ExecuteInEditor
{
	public  CameraComponent ViewModelCamera;

	/// <summary>
	/// The current camera controller for this player.
	/// </summary>
	[RequireComponent] public CameraController CameraController { get; set; }
	
	[Property] public GameObject Head { get; set; }
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
	[Property] public SkinnedModelRenderer PlayerBody;
	[Property] public CharacterController CharacterController { get; set; }

	/// <summary>
	/// A reference to the animation helper (normally on the Body GameObject)
	/// </summary>
	[Property] public AnimationHelper AnimationHelper { get; set; }

	[Property] public virtual HealthSystem HealthSystem { get; set; }
	[Property] public virtual SpotLight SpotLight { get; set; }
	protected BoxCollider Collider;
	[Property] public GameObject Boom { get; set; }

	[Property] public GameObject DefaultPlayerCameraPrefab { get; set; }
	public GameObject PlayerCameraGameObject { get; set; }

	public Inventory Inventory { get; private set; }


	/// <summary>
	/// Is the player holding use?
	/// </summary>
	[Sync] public bool IsUsing { get; set; }
	protected override void OnStart()
	{
		// ViewModelCamera = Scene.GetAllComponents<CameraComponent>().Where( x => x.IsMainCamera ).FirstOrDefault();
		// ViewModelCamera = Components.GetOrCreate<CameraComponent>( FindMode.InChildren );

		PlayerBody = Components.Get<SkinnedModelRenderer>( FindMode.EverythingInSelfAndDescendants );
		// Collider = Components.Get<BoxCollider>( FindMode.EverythingInSelfAndDescendants );

		Inventory = Components.Get<Inventory>( FindMode.EverythingInSelfAndDescendants );;
		SpotLight = GameObject.Components.Get<SpotLight>( FindMode.InChildren );
	}

	protected override void OnUpdate()
	{
		if ( !IsProxy )
		{
			// MouseInput();
			// Transform.Rotation = new Angles( 0, EyeAngles.yaw, 0 );
		}

		OnUpdateMovement();

		CrouchAmount = CrouchAmount.LerpTo( IsCrouching ? 1 : 0, Time.Delta * CrouchLerpSpeed() );
		_smoothEyeHeight = _smoothEyeHeight.LerpTo( _eyeHeightOffset * (IsCrouching ? CrouchAmount : 1), Time.Delta * 10f );
		CharacterController.Height = Height + _smoothEyeHeight;

		// UpdateAnimation();
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy )
			return;

		// CrouchingInput();
		// MovementInput();
		// UpdateAttack();

		UpdateStamina();
		RestoreStamina();
		UpdateInteractions();

		var cc = CharacterController;
		if ( !cc.IsValid() ) return;

		var wasGrounded = IsGrounded;
		IsGrounded = cc.IsOnGround;

		if ( IsGrounded != wasGrounded )
		{
			GroundedChanged( wasGrounded, IsGrounded );
		}

		UpdateEyes();
		// UpdateZones();
		// UpdateOutline();

		// if ( IsViewer )
		// {
		// 	CachedEyeTrace = Scene.Trace.Ray( AimRay, 100000f )
		// 		.IgnoreGameObjectHierarchy( GameObject )
		// 		.WithoutTags( "ragdoll", "movement" )
		// 		.UseHitboxes()
		// 		.Run();
		// }

		if ( HealthSystem.IsDead )
		{
			return;
		}

		// if ( Networking.IsHost && PlayerState.IsBot )
		// {
		// 	if ( BotFollowHostInput )
		// 	{
		// 		BuildWishInput();
		// 		BuildWishVelocity();
		// 	}

		// 	// If we're a bot call these so they don't float in the air.
		// 	ApplyAcceleration();
		// 	ApplyMovement();
		// 	return;
		// }

		if ( !IsLocallyControlled )
		{
			return;
		}

		_previousVelocity = cc.Velocity;

		// UpdatePlayArea();
		// UpdateUse();
		BuildWishInput();
		BuildWishVelocity();
		BuildInput();

		// UpdateRecoilAndSpread();
		ApplyAcceleration();
		ApplyMovement();
	}
	protected override void OnPreRender()
	{
		UpdateBodyVisibility();
		
		if ( IsProxy )
			return;

		// UpdateCamera();

		SpotLight.Transform.Rotation = CameraController.Camera.Transform.Rotation;
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

	private GameObject GetOrCreateCameraObject()
	{
		var component = Scene.GetAllComponents<PlayerCameraOverride>().FirstOrDefault();

		var config = new CloneConfig()
		{
			StartEnabled = true,
			Parent = Boom,
			Transform = new Transform()
		};

		if ( component.IsValid() )
			return component.Prefab.Clone( config );

		return DefaultPlayerCameraPrefab?.Clone( config );
	}

}
