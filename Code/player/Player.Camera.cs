using Sandbox;
using Sandbox.Citizen;
using Sandbox.Diagnostics;
using System;
using System.Linq;
using System.Numerics;


public partial class Player : Component
{
	[Property] public float WheelSpeed => 30f;
	[Property] public Vector2 CameraDistance => new( 500, 1300 );
	[Property] public Vector2 PitchClamp => new( 45, 45 );
	public float Zoom { get; set; } = 30f;

	[Sync]
	public Angles EyeAngles { get; set; }

	public Vector2 CursorScreenPosition { get; set; }
	float OrbitDistance = 500f;
	float TargetOrbitDistance = 600f;
	Angles OrbitAngles = Angles.Zero;

	protected void UpdateCamera()
	{
		if ( Camera == null )
			return;

		if ( DebugCamera )
		{
			DebugCameraPosition += Camera.Transform.Rotation * Input.AnalogMove * Time.Delta * ((Input.Down( InputAction.Sprint ) ? 1000f : (Input.Down( InputAction.Walk ) ? 150f : 400f)));
			Camera.Transform.Position = Camera.Transform.Position.LerpTo( DebugCameraPosition, Time.Delta * 5f );
			Camera.Transform.Rotation = Rotation.Lerp( Camera.Transform.Rotation, DebugCameraAngles, Time.Delta * 5f );
		}
		else
		{
			var eyes = EyePos.Transform;
			var rot = Transform.Rotation;
			var oldEyeRot = Camera.Transform.Rotation;
			var newEyeRot = IsRagdolled ? eyes.Rotation : EyeAngles.ToRotation();
			var oldEyePos = Camera.Transform.Position;
			var newEyePos = eyes.Position + (IsRagdolled ? 0f : rot.Forward * 2.4f);

			Camera.Transform.Position = IsRagdolled ? Vector3.Lerp( oldEyePos, newEyePos, Time.Delta * 10f ) : newEyePos;
			Camera.Transform.Rotation = IsRagdolled ? Rotation.Lerp( oldEyeRot, newEyeRot, Time.Delta * 5f ) : newEyeRot;
			var newRot = Rotation.FromRoll( Vector3.Dot( Transform.Rotation.Right, MoveHelper.Velocity.Normal ) ) * 2f;
			_lastRot = Rotation.Lerp( _lastRot, newRot, Time.Delta * 5f );
			Camera.Transform.Rotation *= _lastRot;
		}

		Camera.FieldOfView = MathX.LerpTo( Camera.FieldOfView, Input.Down( InputAction.Zoom ) ? Zoom : 90f, 10f * Time.Delta );
		Camera.ZNear = 2.5f;

		UpdateHeadVisibility();
	}

	public void UpdateHeadVisibility()
	{
		var eyes = PlayerBody.GetAttachment( "eyes" ) ?? Transform.World;
		var rot = Transform.Rotation;

		if ( HideHead )
			PlayerBody?.SceneModel?.SetBoneWorldTransform( 7,
				new Transform( eyes.Position + rot.Backward * 10, Rotation.Identity, 0 ) );

		// Hide face and head clothing.
		// var face = (Inventory.EquippedItems?.ElementAtOrDefault( (int)EquipSlot.Face ) as ItemEquipment)?.Renderer;
		// if ( face != null )
		// 	face.RenderType = HideHead ? ModelPlayerBody.ShadowRenderType.ShadowsOnly : ModelPlayerBody.ShadowRenderType.On;

		// var head = (Inventory.EquippedItems?.ElementAtOrDefault( (int)EquipSlot.Head ) as ItemEquipment)?.Renderer;
		// if ( head != null )
		// 	head.RenderType = HideHead ? ModelPlayerBody.ShadowRenderType.ShadowsOnly : ModelPlayerBody.ShadowRenderType.On;
	}


	// protected override void OnFixedUpdate()
	// {
	// 	var wheel = Input.MouseWheel;

	// 	if ( wheel.y != 0 )
	// 	{
	// 		TargetOrbitDistance -= wheel.y * WheelSpeed;
	// 		TargetOrbitDistance = TargetOrbitDistance.Clamp( CameraDistance.x, CameraDistance.y );
	// 	}

	// 	OrbitDistance = OrbitDistance.LerpTo( TargetOrbitDistance, Time.Delta * 10f );

	// 	if ( Input.UsingController || Input.Down( "MoveCamera" ) )
	// 	{
	// 		OrbitAngles.yaw += Input.AnalogLook.yaw * 5f;
	// 		OrbitAngles.pitch += Input.AnalogLook.pitch * 5f;
	// 		OrbitAngles = OrbitAngles.Normal;
	// 	}

	// 	EyeAngles = OrbitAngles.WithPitch( 0f );

	// 	OrbitAngles.pitch = OrbitAngles.pitch.Clamp( PitchClamp.x, PitchClamp.y );
	// }
}
