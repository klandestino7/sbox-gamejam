using Sandbox.Citizen;
namespace Gamejam;
public partial class NPC : Component
{
	[Property] public float WalkMoveSpeed { get; set; } = 190.0f;
	[Property] public float RunMoveSpeed { get; set; } = 190.0f;
	[Property] public float SprintMoveSpeed { get; set; } = 320.0f;

	[Sync] public bool Crouching { get; set; }
	[Sync] public Angles EyeAngles { get; set; }
	[Sync] public Vector3 WishVelocity { get; set; }

	public bool IsMoving = false;
    private bool isRunning = false;
	public bool WishCrouch;
	public float EyeHeight = 64;

	float CurrentMoveSpeed { get; set; } = 190f;

	float GetFriction()
	{
		if ( CharacterController.IsOnGround ) return 6.0f;

		// air friction
		return 0.2f;
	}
	int LastTick;

	public void ChekcIsOnGround()
	{
		CharacterController.IsOnGround = true;
	}
	
	public virtual void FollowTarget()
	{
		if ( CharacterController is null )
			return;

		var cc = CharacterController;

		if ( TargetObject == null ) {
			WishVelocity = Vector3.Zero;
			return;
		}

		TargetPosition = TargetObject.Transform.Position;
		if ( TargetPosition == Vector3.Zero ) return;

		var distanceToTarget = Transform.Position.Distance( TargetPosition ) / 2f;

		if (distanceToTarget <= 30f) {
			WishVelocity = Vector3.Zero;
			return;
		}

		var currentTick = (int)(Time.Now / Time.Delta);

		if ( LastTick >= currentTick ) {
			return;
		}

		LastTick = currentTick + 3;

		var targetDir = TargetPosition - Transform.Position;
		var wishDirection = targetDir.Normal; 

		var wishVelocity = wishDirection * CurrentMoveSpeed;
		var steeringForce = wishVelocity - cc.Velocity;

		Rotation targetRot = Rotation.LookAt( targetDir.WithZ( 0f ) );
		Transform.Rotation = Rotation.Lerp( Transform.Rotation, targetRot, Time.Delta * 20f );

		EyeAngles = Transform.Rotation.Angles();
		WishVelocity = wishVelocity;
		WishVelocity += steeringForce;
	}

	public void UpdateMovement()
	{
		if ( CharacterController is null )
			return;

		var cc = CharacterController;
		
		cc.Accelerate( WishVelocity );
		cc.Velocity = CharacterController.Velocity.WithZ( 0 );
		cc.ApplyFriction( GetFriction() );
		cc.Move();

		ChekcIsOnGround();
	}

	private void UpdateAnimation()
	{
		if ( AnimationHelper is null ) return;

		var wv = WishVelocity.Length;

		AnimationHelper.WithWishVelocity( WishVelocity );
		AnimationHelper.WithVelocity( CharacterController.Velocity );
		// AnimationHelper.IsGrounded = CharacterController.IsOnGround;

		AnimationHelper.MoveStyle = wv < 160f ? CitizenAnimationHelper.MoveStyles.Walk : CitizenAnimationHelper.MoveStyles.Run;

		var lookDir = EyeAngles.ToRotation().Forward * 1024;
		AnimationHelper.WithLook( lookDir, 1, 0.5f, 0.25f );
	}
}
