// using Sandbox;
// using Sandbox.Citizen;
// using Sandbox.Diagnostics;
// using System;
// using System.Linq;
// using System.Numerics;

// public partial class Player : Component
// {
// 	[Property] public Vector3 Gravity { get; set; } = new Vector3( 0, 0, 800 );
// 	[Property] public bool FirstPerson { get; set; }

// 		/// <summary>
// 	/// How fast you move when holding the sprint button
// 	/// </summary>
// 	[Property, Sync]
// 	[Category( "Movement" )]
// 	[Range( 0f, 800f, 1f )]
// 	public float SprintSpeed { get; set; } = 280f;

// 	/// <summary>
// 	/// How fast you move when holding the walk button
// 	/// </summary>
// 	[Property, Sync]
// 	[Category( "Movement" )]
// 	[Range( 0f, 200f, 1f )]
// 	public float WalkSpeed { get; set; } = 60f;

// 	/// <summary>
// 	/// How fast you move when holding the duck button
// 	/// </summary>
// 	[Property, Sync]
// 	[Category( "Movement" )]
// 	[Range( 0f, 200f, 1f )]
// 	public float DuckSpeed { get; set; } = 60f;

// 	/// <summary>
// 	/// How high you can jump
// 	/// </summary>
// 	[Property, Sync]
// 	[Category( "Movement" )]
// 	[Range( 0f, 800f, 1f )]
// 	public float JumpStrength { get; set; } = 200f;

// 	public Vector3 MoveDirection { get;  set; }
// 	public const float DUCK_HEIGHT = 58f;
// 	public const float HEIGHT = 72f;
// 	[Sync] public bool Ducking { get; set; }
// 	Rotation _lastRot;

// 	bool _blockMovements = false;
// 	/// <summary>
// 	/// Block both inputs and mouse aiming
// 	/// </summary>
// 	[Sync]
// 	public bool BlockMovements
// 	{
// 		get => DebugCamera ? true : _blockMovements;
// 		set => _blockMovements = value;
// 	}

	
// 	bool _blockMouseAim = false;

// 	/// <summary>
// 	/// Block mouse aiming
// 	/// </summary>
// 	[Sync]
// 	public bool BlockMouseAim
// 	{
// 		get => BlockMovements || _blockMouseAim;
// 		set => _blockMouseAim = value;
// 	}

// 	// public bool IsRunning { get; set; }
// 	// protected override void OnEnabled()
// 	// {
// 	// 	base.OnEnabled();

// 	// 	if ( IsProxy )
// 	// 		return;

// 	// 	var cam = Scene.GetAllComponents<CameraComponent>().FirstOrDefault();
// 	// 	if ( cam is not null )
// 	// 	{
// 	// 		var ee = cam.Transform.Rotation.Angles();
// 	// 		ee.roll = 0;
// 	// 		EyeAngles = ee;
// 	// 	}
// 	// }

// 	protected void ControllerOnUpdate()
// 	{
// 		var movement = Input.AnalogMove.Normal;
// 		var angles = EyeAngles;
// 		var newRotationAngles = new Angles( 0, EyeAngles.yaw, 0 );

// 		if ( Input.Down( "run" ) )
// 			Body.Transform.Rotation = newRotationAngles;
// 		else
// 			Body.Transform.Rotation = Rotation.Lerp(  newRotationAngles , angles.ToRotation(), Time.Delta * 2f);

// 		var isWalking = Input.Down( InputAction.Walk );

// 		var wishSpeed = Ducking ? DuckSpeed : isWalking ? WalkSpeed : SprintSpeed;

// 		var wishVelocity = Input.AnalogMove.Normal * wishSpeed * EyeAngles.WithPitch( 0f );
// 		MoveDirection = wishVelocity;
// 	}

// 	[Broadcast]
// 	public void OnJump( float floatValue, string dataString, object[] objects, Vector3 position )
// 	{
// 		AnimationHelper?.TriggerJump();
// 	}

// 	float fJumps;
// 	protected void UpdateController()
// 	{
// 		if ( IsProxy )
// 			return;

// 		BuildWishVelocity();

// 		var cc = GameObject.Components.Get<CharacterController>();

// 		if ( cc.IsOnGround && Input.Down( "Jump" ) )
// 		{
// 			float flGroundFactor = 1.0f;
// 			float flMul = 268.3281572999747f * 1.2f;

// 			cc.Punch( Vector3.Up * flMul * flGroundFactor );

// 			OnJump( fJumps, "Hello", new object[] { Time.Now.ToString(), 43.0f }, Vector3.Random );

// 			fJumps += 1.0f;
// 		}

// 		if ( cc.IsOnGround )
// 		{
// 			cc.Velocity = cc.Velocity.WithZ( 0 );
// 			cc.Accelerate( WishVelocity );
// 			cc.ApplyFriction( 4.0f );
// 		}
// 		else
// 		{
// 			cc.Velocity -= Gravity * Time.Delta * 0.5f;
// 			cc.Accelerate( WishVelocity.ClampLength( 50 ) );
// 			cc.ApplyFriction( 0.1f );
// 		}

// 		cc.Move();
// 		// MoveHelper.Move();

// 		if ( !cc.IsOnGround )
// 		{
// 			cc.Velocity -= Gravity * Time.Delta * 0.5f;
// 		}
// 		else
// 		{
// 			cc.Velocity = cc.Velocity.WithZ( 0 );
// 		}
// 	}

// 	private Angles _currentRecoil;
// 	private Angles _previousRecoil;
// 	protected void UpdateAngles()
// 	{
// 		if ( DebugCamera )
// 		{
// 			DebugCameraAngles += Input.AnalogLook;
// 		}
// 		if ( BlockMouseAim ) return;

// 		var before = EyeAngles;
// 		var ang = EyeAngles;
// 		ang += Input.AnalogLook;
// 		ang += _currentRecoil - _previousRecoil; // Apply recoil to eye angles.
// 		ang.pitch = ang.pitch.Clamp( -89, 89 );

// 		EyeAngles = ang;

// 		// Calculate recoil.
// 		var diff = (before - ang).AsVector3().Abs().Length.Clamp( 1, 15 );
// 		_previousRecoil = _currentRecoil;
// 		_currentRecoil = _currentRecoil.LerpTo( Angles.Zero, diff * Time.Delta );
// 	}

	
// 	// protected void UpdateMovement()
// 	// {
// 	// 	if ( MoveHelper == null ) return;

// 	// 	var previousFallSpeed = Velocity.z;
// 	// 	var isWalking = Input.Down( InputAction.Walk );

// 	// 	var wishSpeed = Ducking ? DuckSpeed : isWalking ? WalkSpeed : SprintSpeed;
// 	// 	var wishVelocity = Input.AnalogMove.Normal * wishSpeed * EyeAngles.WithPitch( 0f );

// 	// 	// // // Apply encumbarance.
// 	// 	// if ( IsEncumbered )
// 	// 	// {
// 	// 	// 	var overweight = 10f;
// 	// 	// 	var multiplier = MathX.Clamp( 1 - overweight / 15f, 0.2f, 1f );
// 	// 	// 	wishVelocity *= multiplier;
// 	// 	// }

// 	// 	MoveHelper.WishVelocity = BlockInputs ? Vector3.Zero : wishVelocity;

// 	// 	if ( !BlockInputs && Input.Pressed( InputAction.Jump ) && MoveHelper.IsOnGround )
// 	// 	{
// 	// 		PlayerBody?.Set( "jump", true );
// 	// 		// JumpBroadcast();
// 	// 		OnJump( fJumps, "Hello", new object[] { Time.Now.ToString(), 43.0f }, Vector3.Random );
// 	// 	}

// 	// 	// Ducking
// 	// 	var from = Transform.Position + Vector3.Up * 5f;
// 	// 	var to = from + Vector3.Up * (HEIGHT - 5f);

// 	// 	// If we block inputs let's just keep whatever ducking state you were in
// 	// 	if ( !BlockInputs )
// 	// 	{
// 	// 		Ducking = (Ducking && Scene.Trace.Ray( in from, in to ).Size( Collider.Scale.WithZ( 0f ) )
// 	// 					  .IgnoreGameObjectHierarchy( GameObject ).WithoutTags( "trigger" ).Run().Hit)
// 	// 				  || Input.Down( InputAction.Duck ); // Beautiful.
// 	// 	}

// 	// 	MoveHelper.Move();

// 	// 	var diff = MathF.Abs( Velocity.z - previousFallSpeed );
// 	// 	if ( diff > 600 && previousFallSpeed < Velocity.z && !ForceHidePenoid ) // ForceHidePenoid, hack
// 	// 	{
// 	// 		var time = MathX.Clamp( 1, 4, diff / 250f );
// 	// 		// SetRagdoll( true, duration: time );
// 	// 		// GameObject.PlaySound( "impact" );
// 	// 	}

// 	// 	// Update Collider
// 	// 	var height = Ducking ? DUCK_HEIGHT : HEIGHT;
// 	// 	var bbox = MoveHelper.CollisionBBox;

// 	// 	MoveHelper.CollisionBBox = new BBox( bbox.Mins, bbox.Maxs.WithZ( height ) );
// 	// 	Collider.Scale = Collider.Scale.WithZ( height );
// 	// 	Collider.Center = Vector3.Up * height / 2f;
// 	// }


// 	public void BuildWishVelocity()
// 	{
// 		var rot = EyeAngles.ToRotation();

// 		WishVelocity = rot * MoveDirection;
// 		WishVelocity = WishVelocity.WithZ( 0 );
// 		WishVelocity *= Rotation.From( 0f, 0f, 0f );

// 		if ( !WishVelocity.IsNearZeroLength ) WishVelocity = WishVelocity.Normal;

// 		if ( Input.Down( "Run" ) ) WishVelocity *= 320.0f;
// 		else WishVelocity *= 110.0f;
// 	}
// }
