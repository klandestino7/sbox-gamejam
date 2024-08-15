namespace Gamejam;
public partial class Player : Component
{
	float Zoom = 100f;

	public bool IsActive { get; private set; }

	public CameraComponent Camera { get; set; }
	public AudioListener AudioListener { get; set; }
	public ColorAdjustments ColorAdjustments { get; set; }
	// public ScreenShaker ScreenShaker { get; set; }
	public ChromaticAberration ChromaticAberration { get; set; }

	public Pixelate Pixelate { get; set; }

	public void SetActive( bool isActive )
	{
		IsActive = isActive;

		if ( PlayerCameraGameObject.IsValid() )
			PlayerCameraGameObject.Destroy();

		if ( isActive )
		{
			PlayerCameraGameObject = GetOrCreateCameraObject();

			ViewModelCamera = PlayerCameraGameObject.Components.GetOrCreate<CameraComponent>();
			Pixelate = PlayerCameraGameObject.Components.GetOrCreate<Pixelate>();
			ChromaticAberration = PlayerCameraGameObject.Components.GetOrCreate<ChromaticAberration>();
			AudioListener = PlayerCameraGameObject.Components.GetOrCreate<AudioListener>();
			// ScreenShaker = PlayerCameraGameObject.Components.GetOrCreate<ScreenShaker>();

			// Optional
			ColorAdjustments = PlayerCameraGameObject.Components.Get<ColorAdjustments>();
		}

		// OnModeChanged();
		// Boom.Transform.Rotation = Player.EyeAngles.ToRotation();
	}


	protected void UpdateCamera()
	{
		if ( ViewModelCamera == null )
			return;

		
		var oldCamRot = ViewModelCamera.Transform.Rotation;
		var newCamRot = EyeAngles;

		var targetEyeHeight = Crouching ? 28 : 64;
		EyeHeight = EyeHeight.LerpTo( targetEyeHeight, RealTime.Delta * 10.0f );
		var targetCameraPos = Transform.Position + new Vector3( 0, 0, EyeHeight );

		var oldCamPos = ViewModelCamera.Transform.Position;
		var newCamPos = targetCameraPos;

		// smooth view z, so when going up and down stairs or ducking, it's smooth af
		if ( lastUngrounded > 0.2f )
		{
			targetCameraPos.z = ViewModelCamera.Transform.Position.z.LerpTo( targetCameraPos.z, RealTime.Delta * 25.0f );
		}

		ViewModelCamera.Transform.Rotation = Rotation.Lerp( oldCamRot, newCamRot, Time.Delta * 10f );
		ViewModelCamera.Transform.Position =  Vector3.Lerp( oldCamPos, newCamPos, Time.Delta * 15f );
		SpotLight.Transform.Rotation = ViewModelCamera.Transform.Rotation;

		ViewModelCamera.FieldOfView = Preferences.FieldOfView;
	}
}
