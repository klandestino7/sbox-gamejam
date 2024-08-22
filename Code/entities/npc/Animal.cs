using Sandbox;

namespace Gamejam;

public abstract partial class Animal : NPC
{
	protected enum MovementState
	{
		Idle,
		Moving
	}

	// protected RoamBehavior Roam { get; set; }
	// protected WanderBehavior Wander { get; set; }
	// protected AvoidanceBehavior Avoidance { get; set; }
	// protected SteeringComponent Steering { get; set; }

	protected TimeUntil NextChangeState { get; set; }
	protected MovementState State { get; set; }

	protected override void OnStart()
	{
		// Steering = Components.GetOrCreate<SteeringComponent>();
		// Avoidance = Components.GetOrCreate<AvoidanceBehavior>();
		// Wander = Components.GetOrCreate<WanderBehavior>();
		// Roam = Components.GetOrCreate<RoamBehavior>();

		// NextChangeState = Game.Random.Float( 1f, 4f );
		State = MovementState.Idle;

		// base.Spawn();
	}

	protected virtual bool CanChangeState()
	{
		return true;
	}

	
	public void UpdateVelocity()
	{
		// if ( State == MovementState.Idle )
		// {
		// 	Velocity = Vector3.Zero;
		// 	return;
		// }

		// Vector3 acceleration = default;

		// if ( HasValidPath() )
		// {
		// 	var direction = (GetPathTarget() - Position).Normal;
		// 	acceleration += direction * GetMoveSpeed();

		// 	if ( Debug )
		// 	{
		// 		DebugOverlay.Sphere( Position, 16f, Color.Green );
		// 		DebugOverlay.Text( "PATH", Position );
		// 	}
		// }
		// else
		// {
		// 	acceleration += Avoidance.GetSteering();
		// 	acceleration += Wander.GetSteering();
		// }

		// if ( !acceleration.IsNearZeroLength )
		// {
		// 	Steering.Steer( acceleration );
		// }
	}
}