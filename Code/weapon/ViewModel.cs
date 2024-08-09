namespace Gamejam;

public class ViewModel : Component
{
	[Property, Group( "Components" )] public SkinnedModelRenderer ArmsModelRenderer { get; set; }

	[Property, Group( "Components" )] public SkinnedModelRenderer ModelRenderer { get; set; }

	/// <summary>
	/// Looks up the tree to find the player controller.
	/// </summary>
	// private PlayerController PlayerController => Weapon.Components.GetInAncestors<PlayerController>();
	private CameraComponent Camera { get; set; }
	private Weapon Weapon { get; set; }

	/// Looks up the tree to find the player controller.
	/// </summary>
	Player? Owner => this.Weapon.IsValid() ? this.Weapon.Owner : null;

	private float YawInertiaScale => 2f;
	private float PitchInertiaScale => 2f;
	private bool activateInertia = false;
	private float lastPitch;
	private float lastYaw;
	private float YawInertia;
	private float PitchInertia;

	// TODO: FAZER OS OFFSETS, É NECESSARIO!!!!
	// IEnumerable<IViewModelOffset> Offsets => this.Weapon.Components.GetAll<IViewModelOffset>( FindMode.EverythingInSelfAndDescendants );

	public void SetWeapon( Weapon weapon )
	{
		Weapon = weapon;
	}

	public void SetCamera( CameraComponent camera )
	{
		Camera = camera;
	}

	protected override void OnAwake()
	{
		ModelRenderer?.Set( "b_deploy_skip", true );
	}

	protected override void OnStart()
	{
		/*
		if ( IsThrowable )
			ModelRenderer?.Set( "throwable_type", (int)ThrowableType );
		*/

		// Somehow?
		/*
		if ( Owner.IsValid() )
			Owner.OnJump += OnPlayerJumped;
		*/

		// Somehow this can happen?
		if ( !this.Weapon.IsValid() )
			return;

		/*
		if ( this.Weapon.Components.Get<ShootWeaponComponent>( FindMode.EverythingInSelfAndDescendants ) is { } shoot )
		{
			OnFireMode( shoot.CurrentFireMode );
		}
		*/
	}

	void OnPlayerJumped()
	{
		ModelRenderer?.Set( "b_jump", true );
	}

	void ApplyAnimationTransform()
	{
		if ( !ModelRenderer.IsValid() ) return;
		if ( !ModelRenderer.Enabled ) return;

		Player? owner = this.Owner;

		if ( !owner.IsValid() )
		{
			return;
		}

		var bone = ModelRenderer.SceneModel.GetBoneLocalTransform( "camera" );
		var camera = owner.ViewModelCamera.GameObject;

		var scale = /* GameSettingsSystem.Current.ViewBob */ 100.0f / 100f;

		camera.Transform.LocalPosition += bone.Position * scale;
		camera.Transform.LocalRotation *= bone.Rotation * scale;
	}

	void ApplyOffsets()
	{
		// foreach ( var offset in Offsets )
		// {
		// 	// Log.Info( $"Offsetting by {offset.PositionOffset}" );
		// 	localPosition += offset.PositionOffset;
		// 	localRotation *= offset.AngleOffset.ToRotation();
		// }
	}

	void ApplyInertia()
	{
		Player? owner = this.Owner;

		if ( !owner.IsValid() )
		{
			return;
		}

		var camera = owner.ViewModelCamera.GameObject;
		var inRot = camera.Transform.Rotation;

		// Need to fetch data from the camera for the first frame
		if ( !activateInertia )
		{
			lastPitch = inRot.Pitch();
			lastYaw = inRot.Yaw();
			YawInertia = 0;
			PitchInertia = 0;
			activateInertia = true;
		}

		var newPitch = camera.Transform.Rotation.Pitch();
		var newYaw = camera.Transform.Rotation.Yaw();

		PitchInertia = Angles.NormalizeAngle( newPitch - lastPitch );
		YawInertia = Angles.NormalizeAngle( lastYaw - newYaw );

		lastPitch = newPitch;
		lastYaw = newYaw;
	}

	private Vector3 lerpedWishMove;

	private Vector3 localPosition;
	private Rotation localRotation;

	private Vector3 lerpedLocalPosition;
	private Rotation lerpedlocalRotation;

	protected void ApplyVelocity()
	{
		var moveVel = Owner.CharacterController.Velocity;
		var moveLen = moveVel.Length;

		// var wishMove = Owner.WishMove.Normal * 1f;
		// if ( Equipment?.Tags.Has( "aiming" ) ?? false ) wishMove = 0;

		// if ( Owner.IsSlowWalking || Owner.IsCrouching ) moveLen *= 0.5f;

		// lerpedWishMove = lerpedWishMove.LerpTo( wishMove, Time.Delta * 7.0f );
		ModelRenderer?.Set( "move_bob", moveLen.Remap( 0, 300, 0, 1, true ) );

		// if ( UseMovementInertia )
		// 	YawInertia += lerpedWishMove.y * 10f;

		ModelRenderer?.Set( "aim_yaw_inertia", YawInertia * YawInertiaScale );
		ModelRenderer?.Set( "aim_pitch_inertia", PitchInertia * PitchInertiaScale );
	}

	private float FieldOfViewOffset = 0f;
	private float TargetFieldOfView = 90f;

	void ApplyAnimationParameters()
	{
		// ModelRenderer.Set( "b_sprint", Owner.IsSprinting );
		// ModelRenderer.Set( "b_grounded", Owner.IsGrounded );

		// Ironsights
		// ModelRenderer.Set( "ironsights", Equipment.Tags.Has( "aiming" ) ? 1 : 0 );
		// ModelRenderer.Set( "ironsights_fire_scale", Equipment.Tags.Has( "aiming" ) ? IronsightsFireScale : 0f );

		// Handedness
		ModelRenderer.Set( "b_twohanded", true );

		// Weapon state
		// ModelRenderer.Set( "b_empty", !Equipment.Components.Get<AmmoComponent>( FindMode.EnabledInSelfAndDescendants )?.HasAmmo ?? false );
	}
	
	public enum ThrowableTypeEnum
	{
		HEGrenade,
		SmokeGrenade,
		StunGrenade,
		Molotov,
		Flashbang
	}

	/// <summary>
	/// Should we play deploy effects?
	/// </summary>
	public bool PlayDeployEffects
	{
		set
		{
			ModelRenderer?.Set( "b_deploy", value );
			ModelRenderer?.Set( "b_deploy_skip", !value );
		}
	}

	private void ApplyThrowableAnimations()
	{
		// var throwFn = Equipment.Components.Get<ThrowWeaponComponent>( FindMode.EnabledInSelfAndDescendants );

		// ModelRenderer.Set( "b_idle", throwFn.ThrowState == ThrowWeaponComponent.State.Idle );
		// ModelRenderer.Set( "b_pull", throwFn.ThrowState == ThrowWeaponComponent.State.Cook );
		// ModelRenderer.Set( "b_throw", throwFn.ThrowState == ThrowWeaponComponent.State.Throwing );
	}

	protected override void OnUpdate()
	{
		// Reset every frame
		localRotation = Rotation.Identity;
		localPosition = Vector3.Zero;

		if ( !Owner.IsValid() || !Owner.CharacterController.IsValid() )
			return;

		// if ( IsThrowable )
		// {
		// 	ApplyThrowableAnimations();
		// }
		// else
		// {
			ApplyAnimationParameters();
		// }
		
		ApplyVelocity();
		ApplyAnimationTransform();
		ApplyInertia();
		ApplyOffsets();

		var baseFov = 60.0f; // GameSettingsSystem.Current.FieldOfView;

		TargetFieldOfView = TargetFieldOfView.LerpTo( baseFov + FieldOfViewOffset, Time.Delta * 10f );
		FieldOfViewOffset = 0;

		lerpedlocalRotation = Rotation.Lerp( lerpedlocalRotation, localRotation, Time.Delta * 10f );
		lerpedLocalPosition = lerpedLocalPosition.LerpTo( localPosition, Time.Delta * 10f );

		Transform.LocalRotation = lerpedlocalRotation;
		Transform.LocalPosition = lerpedLocalPosition;
	}

	// public void OnFireMode( FireMode currentFireMode )
	// {
	// 	var mode = currentFireMode switch
	// 	{
	// 		FireMode.Semi => 1,
	// 		FireMode.Automatic => 3,
	// 		FireMode.Burst => 2,
	// 		_ => 0
	// 	};

	// 	ModelRenderer.Set( "firing_mode", mode );
	// }
}
