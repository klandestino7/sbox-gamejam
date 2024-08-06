namespace Gamejam;
public partial class Player : Component
{
	float Zoom = 100f;



	protected void UpdateCamera()
	{
		if ( Camera == null )
			return;

		
		var oldCamRot = Camera.Transform.Rotation;
		var newCamRot = EyeAngles;

		var targetEyeHeight = Crouching ? 28 : 64;
		EyeHeight = EyeHeight.LerpTo( targetEyeHeight, RealTime.Delta * 10.0f );
		var targetCameraPos = Transform.Position + new Vector3( 0, 0, EyeHeight );

		var oldCamPos = Camera.Transform.Position;
		var newCamPos = targetCameraPos;

		// smooth view z, so when going up and down stairs or ducking, it's smooth af
		if ( lastUngrounded > 0.2f )
		{
			targetCameraPos.z = Camera.Transform.Position.z.LerpTo( targetCameraPos.z, RealTime.Delta * 25.0f );
		}

		Camera.Transform.Rotation = Rotation.Lerp( oldCamRot, newCamRot, Time.Delta * 10f );
		Camera.Transform.Position =  Vector3.Lerp( oldCamPos, newCamPos, Time.Delta * 15f );
		SpotLight.Transform.Rotation = Camera.Transform.Rotation;

		Camera.FieldOfView = Preferences.FieldOfView;
	}
}
