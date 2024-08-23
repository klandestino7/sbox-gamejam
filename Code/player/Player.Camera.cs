namespace Gamejam;


public partial class Player : Component
{
	// /// <summary>
	// /// The current camera controller for this player.
	// /// </summary>
	// [RequireComponent] public CameraController CameraController { get; set; }
	// float Zoom = 100f;

	// private CameraMode _mode;
	// public CameraMode Mode
	// {
	// 	get => _mode;
	// 	set
	// 	{
	// 		if ( _mode == value )
	// 			return;

	// 		_mode = value;
	// 		OnModeChanged();
	// 	}
	// }


	// /// <summary>
	// /// Constructs a ray using the camera's GameObject
	// /// </summary>
	// public Ray AimRay
	// {
	// 	get
	// 	{
	// 		if ( Camera.IsValid() )
	// 		{
	// 			return new( Camera.Transform.Position + Camera.Transform.Rotation.Forward, Camera.Transform.Rotation.Forward );
	// 		}

	// 		return new( Transform.Position + Vector3.Up * 64f, Player.EyeAngles.ToRotation().Forward );
	// 	}
	// }

	// public bool IsActive { get; private set; }

	// public override CameraComponent Camera => CameraController.Camera;
	// public AudioListener AudioListener { get; set; }
	// public ColorAdjustments ColorAdjustments { get; set; }
	// // public ScreenShaker ScreenShaker { get; set; }
	// public ChromaticAberration ChromaticAberration { get; set; }
	// [Property] public float ThirdPersonDistance { get; set; } = 128f;
	// [Property] public float AimFovOffset { get; set; } = -5f;
	// public Pixelate Pixelate { get; set; }
	// public float MaxBoomLength { get; set; }
	// [Property, Group( "Config" )] public bool ShouldViewBob { get; set; } = true;
	// [Property, Group( "Config" )] public float RespawnProtectionSaturation { get; set; } = 0.25f;

	// // public void SetActive( bool isActive )
	// // {
	// // 	IsActive = isActive;

	// // 	if ( PlayerCameraGameObject.IsValid() )
	// // 		PlayerCameraGameObject.Destroy();

	// // 	if ( isActive )
	// // 	{
	// // 		PlayerCameraGameObject = GetOrCreateCameraObject();

	// // 		ViewModelCamera = PlayerCameraGameObject.Components.GetOrCreate<CameraComponent>();
	// // 		Pixelate = PlayerCameraGameObject.Components.GetOrCreate<Pixelate>();
	// // 		ChromaticAberration = PlayerCameraGameObject.Components.GetOrCreate<ChromaticAberration>();
	// // 		AudioListener = PlayerCameraGameObject.Components.GetOrCreate<AudioListener>();
	// // 		// ScreenShaker = PlayerCameraGameObject.Components.GetOrCreate<ScreenShaker>();

	// // 		// Optional
	// // 		ColorAdjustments = PlayerCameraGameObject.Components.Get<ColorAdjustments>();
	// // 	}

	// // 	// OnModeChanged();
	// // 	// Boom.Transform.Rotation = Player.EyeAngles.ToRotation();
	// // }

	// /// <summary>
	// /// Updates the camera's position, from player code
	// /// </summary>
	// /// <param name="eyeHeight"></param>
	// internal void UpdateFromEyes( float eyeHeight )
	// {
	// 	if ( !ViewModelCamera.IsValid() )
	// 		return;

	// 	// All transform effects are additive to camera local position, so we need to reset it before anything is applied
	// 	ViewModelCamera.Transform.LocalPosition = Vector3.Zero;
	// 	ViewModelCamera.Transform.LocalRotation = Rotation.Identity;

	// 	if ( Mode == CameraMode.ThirdPerson ) //  && !IsLocallyControlled
	// 	{
	// 		// orbit cam: spectating only
	// 		var angles = Boom.Transform.Rotation.Angles();
	// 		angles += Input.AnalogLook;
	// 		Boom.Transform.Rotation = angles.WithPitch( angles.pitch.Clamp( -90, 90 ) ).ToRotation();
	// 	}
	// 	else
	// 	{
	// 		Boom.Transform.Rotation = EyeAngles.ToRotation();
	// 	}

	// 	if ( MaxBoomLength > 0 )
	// 	{
	// 		var tr = Scene.Trace.Ray( new Ray( Boom.Transform.Position, Boom.Transform.Rotation.Backward ), MaxBoomLength )
	// 			.IgnoreGameObjectHierarchy( GameObject.Root )
	// 			.WithoutTags( "trigger", "player", "ragdoll" )
	// 			.Run();

	// 		ViewModelCamera.Transform.LocalPosition = Vector3.Backward * (tr.Hit ? tr.Distance - 5.0f : MaxBoomLength);
	// 	}

	// 	if ( ShouldViewBob )
	// 	{
	// 		ViewBob();
	// 	}

	// 	Update( eyeHeight );
	// }

	// protected void UpdateCamera()
	// {
	// 	if ( ViewModelCamera == null )
	// 		return;

		
	// 	var oldCamRot = ViewModelCamera.Transform.Rotation;
	// 	var newCamRot = EyeAngles;

	// 	// var targetEyeHeight = Crouching ? 28 : 64;
	// 	// EyeHeight = EyeHeight.LerpTo( targetEyeHeight, RealTime.Delta * 10.0f );
	// 	// var targetCameraPos = Transform.Position + new Vector3( 0, 0, EyeHeight );

	// 	var oldCamPos = ViewModelCamera.Transform.Position;
	// 	var newCamPos = targetCameraPos;

	// 	// smooth view z, so when going up and down stairs or ducking, it's smooth af
	// 	// if ( lastUngrounded > 0.2f )
	// 	// {
	// 	// 	targetCameraPos.z = ViewModelCamera.Transform.Position.z.LerpTo( targetCameraPos.z, RealTime.Delta * 25.0f );
	// 	// }

	// 	ViewModelCamera.Transform.Rotation = Rotation.Lerp( oldCamRot, newCamRot, Time.Delta * 10f );
	// 	ViewModelCamera.Transform.Position =  Vector3.Lerp( oldCamPos, newCamPos, Time.Delta * 15f );
	// 	SpotLight.Transform.Rotation = ViewModelCamera.Transform.Rotation;

	// 	ViewModelCamera.FieldOfView = Preferences.FieldOfView;
	// }

	// float walkBob = 0;
	// private float LerpBobSpeed = 0;

	// // [DeveloperCommand( "Toggle Third Person", "Player" )]
	// // public static void ToggleThirdPerson()
	// // {
	// // 	var pl = PlayerState.Local.PlayerPawn;
	// // 	pl.CameraController.Mode = pl.CameraController.Mode == CameraMode.FirstPerson ? CameraMode.ThirdPerson : CameraMode.FirstPerson;
	// // }
	
	// /// <summary>
	// /// Bob the view!
	// /// This could be better, but it doesn't matter really.
	// /// </summary>
	// void ViewBob()
	// {
	// 	if ( Mode != CameraMode.FirstPerson )
	// 		return;

	// 	var bobSpeed = CharacterController.Velocity.Length.LerpInverse( 0, 300 );
	// 	// if ( !IsGrounded ) bobSpeed *= 0.1f;
	// 	if ( !IsSprinting ) bobSpeed *= 0.3f;

	// 	LerpBobSpeed = LerpBobSpeed.LerpTo( bobSpeed, Time.Delta * 10f );

	// 	walkBob += Time.Delta * 10.0f * LerpBobSpeed;
	// 	var yaw = MathF.Sin( walkBob ) * 0.5f;
	// 	var pitch = MathF.Cos( -walkBob * 2f ) * 0.5f;

	// 	Boom.Transform.LocalRotation *= Rotation.FromYaw( -yaw * LerpBobSpeed );
	// 	Boom.Transform.LocalRotation *= Rotation.FromPitch( -pitch * LerpBobSpeed * 0.5f );
	// }
	// private float FieldOfViewOffset = 0f;
	// private float TargetFieldOfView = 90f;

	// public void AddFieldOfViewOffset( float degrees )
	// {
	// 	FieldOfViewOffset -= degrees;
	// }

	// bool fetchedInitial = false;
	// float defaultSaturation = 1f;

	// private void Update( float eyeHeight )
	// {
	// 	var baseFov = Preferences.FieldOfView;
	// 	FieldOfViewOffset = 0;

	// 	if ( !PlayerBody.IsValid() )
	// 		return;

	// 	// deathcam, "zoom" at target.
	// 	if ( HealthSystem.IsDead )
	// 	{
	// 		FieldOfViewOffset += AimFovOffset; 
	// 	}

	// 	if ( ColorAdjustments.IsValid() )
	// 	{
	// 		if ( !fetchedInitial )
	// 		{
	// 			defaultSaturation = ColorAdjustments.Saturation;
	// 			fetchedInitial = true;
	// 		}

	// 		ColorAdjustments.Saturation = HealthSystem.Invincible
	// 			? RespawnProtectionSaturation
	// 			: ColorAdjustments.Saturation.MoveToLinear( defaultSaturation, 1f );
	// 	}

	// 	// ApplyRecoil();
	// 	// ApplyScope();

	// 	Boom.Transform.LocalPosition = Vector3.Zero.WithZ( eyeHeight );

	// 	ApplyCameraEffects();
	// 	// ScreenShaker?.Apply( Camera );

	// 	TargetFieldOfView = TargetFieldOfView.LerpTo( baseFov + FieldOfViewOffset, Time.Delta * 5f );
	// 	ViewModelCamera.FieldOfView = TargetFieldOfView;
	// }

	// RealTimeSince TimeSinceDamageTaken = 1;

	// void ApplyCameraEffects()
	// {
	// 	var timeSinceDamage = TimeSinceDamageTaken.Relative;
	// 	var shortDamageUi = timeSinceDamage.LerpInverse( 0.1f, 0.0f, true );
	// 	ChromaticAberration.Scale = shortDamageUi * 1f;
	// 	Pixelate.Scale = shortDamageUi * 0.2f;
	// }
	// void OnModeChanged()
	// {
	// 	// SetBoomLength( Mode == CameraMode.FirstPerson ? 0.0f : ThirdPersonDistance );

	// 	// var firstPersonPOV = Mode == CameraMode.FirstPerson && IsActive;
	// 	// Player.Body?.SetFirstPersonView( firstPersonPOV );

	// 	// if ( firstPersonPOV )
	// 	// 	Player.CreateViewModel( false );
	// 	// else
	// 	// 	Player.ClearViewModel();
	// }
}
