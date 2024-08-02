// using Sandbox;
// using Sandbox.Citizen;
// using Sandbox.Diagnostics;
// using System;
// using System.Linq;
// using System.Numerics;

// public partial class Player : Component
// {
// 	// private Weapon LastWeaponEntity { get; set; }
// 	[Property] public GameObject Body { get; set; }
// 	[Property] public CitizenAnimationHelper AnimationHelper { get; set; }

// 	public bool IsRunning { get; set; }

// 	protected void UpdateAnimation()
// 	{
//         Rotation bodyRotation = PlayerBody.Transform.Rotation;
// 		var cc = GameObject.Components.Get<CharacterController>();
// 		// // Eye input
// 		// if ( !IsProxy )
// 		// {
// 		// 	IsRunning = Input.Down( "Run" );
// 		// }

// 		// // rotate body to look angles
// 		// if ( PlayerBody is not null )
// 		// {
// 		// 	// var cursorPos = Components.Get<WorldCursor>( FindMode.Enabled );
// 		// 	var targetAngle = new Angles( 0, EyeAngles.yaw, 0 ).ToRotation();

// 		// 	var v = cc.Velocity.WithZ( 0 );

// 		// 	targetAngle = Rotation.LookAt( v, Vector3.Up );

// 		// 	rotateDifference = PlayerBody.Transform.Rotation.Distance( targetAngle );

// 		// 	// Rotation playerRotation = Rotation.LookAt( cc.Transform.Position, cursorPos.CursorInstance.Transform.Position );

// 		// 	if ( rotateDifference > 50.0f || cc.Velocity.Length > 1.0f )
// 		// 	{
// 		// 		PlayerBody.Transform.Rotation = Rotation.Lerp( PlayerBody.Transform.Rotation, targetAngle, Time.Delta * 2.0f );
// 		// 	}

//         //     if ( aimButtonPressed )
//         //     {
// 		// 		// Body.Transform.Rotation = Rotation.Lerp( Body.Transform.Rotation, playerRotation, Time.Delta * 2.0f );
//         //     }
// 		// }
	
// 		// // Eye input
// 		// if ( cc.IsValid() )
// 		// {
// 		// 	EyeAngles += Input.AnalogLook;
// 		// 	EyeAngles = playerController.EyeAngles.WithPitch( EyeAngles.pitch.Clamp( -90, 90 ) );
// 		// }

// 		// if ( Body.IsValid() )
// 		// {
// 		// 	Body.Transform.Rotation = Rotation.FromYaw( EyeAngles.yaw );
// 		// }

//         if ( AnimationHelper is not null )
//         {
// 			AnimationHelper.Target.SetBodyGroup("head", 1);
//             AnimationHelper.WithVelocity( cc.Velocity );
//             AnimationHelper.WithWishVelocity( WishVelocity );
//             AnimationHelper.IsGrounded = cc.IsOnGround;
//             // AnimationHelper.FootShuffle = rotateDifference;
//             AnimationHelper.AimAngle = bodyRotation;;
//             AnimationHelper.WithLook( EyeAngles.Forward, 1, 1, 1.0f );
//             AnimationHelper.MoveStyle = IsRunning ? CitizenAnimationHelper.MoveStyles.Run : CitizenAnimationHelper.MoveStyles.Walk;
//         }
// 	}

// 	// protected override void OnFixedUpdate()
// 	// {
		
// 	// }
// }
