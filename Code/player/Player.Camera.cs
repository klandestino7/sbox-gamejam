namespace Gamejam;
public partial class Player : Component
{
	float Zoom = 100f;

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
