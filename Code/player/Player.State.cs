using Sandbox.Diagnostics;
using Sandbox.Events;

namespace Gamejam;

public record OnPlayerRagdolledEvent : IGameEvent
{
	public float DestroyTime { get; set; } = 0f;
}

public partial class Player
{

	public void SetSpawnPoint( SpawnPointInfo spawnPoint )
	{
		SpawnPosition = spawnPoint.Position;
		SpawnRotation = spawnPoint.Rotation;

		// SpawnPointTags.Clear();

		foreach ( var tag in spawnPoint.Tags )
		{
			// SpawnPointTags.Add( tag );
		}
	}

	public void OnRespawn()
	{
		Assert.True( Networking.IsHost );

		OnHostRespawn();
		OnClientRespawn();

		
	}

	private void OnHostRespawn()
	{
		Assert.True( Networking.IsHost );

		// _previousVelocity = Vector3.Zero;

		Teleport( SpawnPosition, SpawnRotation );

		// if ( PlayerBody is not null )
		// {
		// 	PlayerBody.DamageTakenForce = Vector3.Zero;
		// }

		HealthSystem.Health = HealthSystem.MaxHealth;

		// TimeSinceLastRespawn = 0f;

		ResetBody();
		Scene.Dispatch( new PlayerSpawnedEvent( this ) );
	}

	[Authority]
	private void OnClientRespawn()
	{

		Possess();
	}

	public void Teleport( Transform transform )
	{
		Teleport( transform.Position, transform.Rotation );
	}

	[Authority]
	public void Teleport( Vector3 position, Rotation rotation )
	{
		Transform.World = new( position, rotation );
		Transform.ClearInterpolation();
		EyeAngles = rotation.Angles();

		if ( CharacterController.IsValid() )
		{
			CharacterController.Velocity = Vector3.Zero;
			CharacterController.IsOnGround = true;
		}
	}

	[Broadcast( NetPermission.HostOnly )]
	private void CreateRagdoll()
	{
		if ( !PlayerBody.IsValid() )
			return;

		// PlayerBody.SetRagdoll( true );
		PlayerBody.GameObject.SetParent( null, true );
		// PlayerBody.GameObject.Name = $"Ragdoll ({DisplayName})";

		var ev = new OnPlayerRagdolledEvent();
		Scene.Dispatch( ev );

		// if ( ev.DestroyTime > 0f )
		// {
		// 	var comp = PlayerBody.Components.Create<TimedDestroyComponent>();
		// 	comp.Time = ev.DestroyTime;
		// }
		// else
		// {
		// 	PlayerBody.Components.Create<DestroyBetweenRounds>();
		// }

		PlayerBody = null;
	}

	private void ResetBody()
	{
		if ( PlayerBody is not null )
		{
			// PlayerBody.DamageTakenForce = Vector3.Zero;
		}

		// PlayerBoxCollider.Enabled = true;

		// Components.Get<HumanOutfitter>( FindMode.EnabledInSelfAndDescendants )?
		// 	.UpdateFromTeam( Team );
	}

	public void OnPossess()
	{
		CameraController.SetActive( true );

		// // if we're spectating a remote player, use the camera mode preference
		// // otherwise: first person for now
		// var spectateSystem = SpectateSystem.Instance;
		// if ( spectateSystem is not null && (IsProxy || PlayerState.IsBot) )
		// {
		// 	CameraController.Mode = spectateSystem.CameraMode;
		// }
		// else
		// {
			CameraController.Mode = CameraMode.FirstPerson;
		// }
	}

	public void OnDePossess()
	{
		CameraController.SetActive( false );
	}
}
