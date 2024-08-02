using Sandbox;
using Sandbox.Citizen;
using Sandbox.Diagnostics;
using System;
using System.Linq;
using System.Numerics;


public partial class Player : Component
{
	protected void UpdateCamera()
	{
		// if ( Camera == null )
		// 	return;
		// var cc = GameObject.Components.Get<CharacterController>();

		// if ( DebugCamera )
		// {
		// 	DebugCameraPosition += Camera.Transform.Rotation * Input.AnalogMove * Time.Delta * ((Input.Down( InputAction.Sprint ) ? 1000f : (Input.Down( InputAction.Walk ) ? 150f : 400f)));
		// 	Camera.Transform.Position = Camera.Transform.Position.LerpTo( DebugCameraPosition, Time.Delta * 5f );
		// 	Camera.Transform.Rotation = Rotation.Lerp( Camera.Transform.Rotation, DebugCameraAngles, Time.Delta * 5f );
		// }
		// else
		// {
		// 	var eyes = EyePos.Transform;
		// 	var rot = Transform.Rotation;
		// 	var oldEyeRot = Camera.Transform.Rotation;
		// 	var newEyeRot = IsRagdolled ? eyes.Rotation : EyeAngles.ToRotation();
		// 	var oldEyePos = Camera.Transform.Position;
		// 	var newEyePos = eyes.Position + (IsRagdolled ? 0f : rot.Forward * 2.4f);

		// 	Camera.Transform.Position = IsRagdolled ? Vector3.Lerp( oldEyePos, newEyePos, Time.Delta * 10f ) : newEyePos;
		// 	Camera.Transform.Rotation = IsRagdolled ? Rotation.Lerp( oldEyeRot, newEyeRot, Time.Delta * 5f ) : newEyeRot;
		// 	var newRot = Rotation.FromRoll( Vector3.Dot( Transform.Rotation.Right, cc.Velocity.Normal ) ) * 2f;
		// 	_lastRot = Rotation.Lerp( _lastRot, newRot, Time.Delta * 5f );
		// 	Camera.Transform.Rotation *= _lastRot;
		// }

		// Camera.FieldOfView = MathX.LerpTo( Camera.FieldOfView, Input.Down( InputAction.Zoom ) ? Zoom : 90f, 10f * Time.Delta );
		// Camera.ZNear = 2.5f;

		// UpdateHeadVisibility();

        var camera = Scene.GetAllComponents<CameraComponent>().Where( x => x.IsMainCamera ).FirstOrDefault();
		if ( camera is null ) return;

		var targetEyeHeight = Crouching ? 28 : 64;
		EyeHeight = EyeHeight.LerpTo( targetEyeHeight, RealTime.Delta * 10.0f );

		var targetCameraPos = Transform.Position + new Vector3( 0, 0, EyeHeight );

		// smooth view z, so when going up and down stairs or ducking, it's smooth af
		if ( lastUngrounded > 0.2f )
		{
			targetCameraPos.z = camera.Transform.Position.z.LerpTo( targetCameraPos.z, RealTime.Delta * 25.0f );
		}

		camera.Transform.Position = targetCameraPos;
		camera.Transform.Rotation = EyeAngles;
		camera.FieldOfView = Preferences.FieldOfView;
	}
}
