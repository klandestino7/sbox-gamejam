using Sandbox;
using Sandbox.Citizen;
using Sandbox.Diagnostics;
using System;
using System.Linq;
using System.Numerics;

public partial class Player : Component
{
	[Property] public Vector3 Gravity { get; set; } = new Vector3( 0, 0, 800 );
	[Property] public bool FirstPerson { get; set; }
	[Property][Category( "Movement" )] public MoveHelper MoveHelper { get; set; }

	public Vector3 MoveDirection { get;  set; }
	Rotation _lastRot;

	// public bool IsRunning { get; set; }
	// protected override void OnEnabled()
	// {
	// 	base.OnEnabled();

	// 	if ( IsProxy )
	// 		return;

	// 	var cam = Scene.GetAllComponents<CameraComponent>().FirstOrDefault();
	// 	if ( cam is not null )
	// 	{
	// 		var ee = cam.Transform.Rotation.Angles();
	// 		ee.roll = 0;
	// 		EyeAngles = ee;
	// 	}
	// }

	protected void ControllerOnUpdate()
	{
		var movement = Input.AnalogMove.Normal;
		var angles = EyeAngles;
		var moveVector = Rotation.From( angles ) * movement * 320f;

		MoveDirection = moveVector;

		if ( Input.Down( "run" ) )
			Body.Transform.Rotation = Body.Transform.Rotation;
		else
			Body.Transform.Rotation = Rotation.Lerp( Body.Transform.Rotation, angles.ToRotation(), Time.Delta * 2f);
	}

	[Broadcast]
	public void OnJump( float floatValue, string dataString, object[] objects, Vector3 position )
	{
		AnimationHelper?.TriggerJump();
	}

	float fJumps;
	protected void UpdateController()
	{
		if ( IsProxy )
			return;

		BuildWishVelocity();

		var cc = GameObject.Components.Get<CharacterController>();

		if ( cc.IsOnGround && Input.Down( "Jump" ) )
		{
			float flGroundFactor = 1.0f;
			float flMul = 268.3281572999747f * 1.2f;

			cc.Punch( Vector3.Up * flMul * flGroundFactor );

			OnJump( fJumps, "Hello", new object[] { Time.Now.ToString(), 43.0f }, Vector3.Random );

			fJumps += 1.0f;
		}

		if ( cc.IsOnGround )
		{
			cc.Velocity = cc.Velocity.WithZ( 0 );
			cc.Accelerate( WishVelocity );
			cc.ApplyFriction( 4.0f );
		}
		else
		{
			cc.Velocity -= Gravity * Time.Delta * 0.5f;
			cc.Accelerate( WishVelocity.ClampLength( 50 ) );
			cc.ApplyFriction( 0.1f );
		}

		cc.Move();
		MoveHelper.Move();

		if ( !cc.IsOnGround )
		{
			cc.Velocity -= Gravity * Time.Delta * 0.5f;
		}
		else
		{
			cc.Velocity = cc.Velocity.WithZ( 0 );
		}

	}

	public void BuildWishVelocity()
	{
		var rot = EyeAngles.ToRotation();

		WishVelocity = rot * MoveDirection;
		WishVelocity = WishVelocity.WithZ( 0 );
		WishVelocity *= Rotation.From( 0f, -70f, 0f );

		if ( !WishVelocity.IsNearZeroLength ) WishVelocity = WishVelocity.Normal;

		if ( Input.Down( "Run" ) ) WishVelocity *= 320.0f;
		else WishVelocity *= 110.0f;
	}
}
