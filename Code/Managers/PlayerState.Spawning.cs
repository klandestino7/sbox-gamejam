using Sandbox.Events;

namespace Gamejam;

public enum RespawnState
{
	Not,
	Requested,
	Delayed,
	Immediate
}

public partial class PlayerState
{
	/// <summary>
	/// The prefab to spawn when we want to make a player pawn for the player.
	/// </summary>
	[Property] public GameObject PlayerPawnPrefab { get; set; }

	public TimeSince TimeSinceRespawnStateChanged { get; private set; }
	public DamageInfo LastDamageInfo { get; private set; }

	/// <summary>
	/// Are we ready to respawn?
	/// </summary>
	// [HostSync, Change( nameof( OnRespawnStateChanged ))] public RespawnState RespawnState { get; set; }

	// public bool IsRespawning => RespawnState is RespawnState.Delayed;

	private void Spawn( SpawnPointInfo spawnPoint )
	{
		Log.Info(" PlayerState :: Spawn ");

		var gameobject = PlayerPawnPrefab.Clone( spawnPoint.Transform );
		var pawn = gameobject.Components.Get<Player>();

		pawn.PlayerState = this;

		// pawn.SetSpawnPoint( spawnPoint );

		gameobject.NetworkSpawn( Network.OwnerConnection );
		gameobject.Flags |= GameObjectFlags.DontDestroyOnLoad;

		Player = pawn;
				
		// RespawnState = RespawnState.Not;
		pawn.OnRespawn();
	}

	public void Respawn( bool forceNew )
	{
		Log.Info(" PlayerState :: Respawn 1");
		var spawnPoint = new SpawnPointInfo();
		Log.Info(" PlayerState :: Respawn 2");

		// Log.Info( $"Spawning player.. ( {GameObject.Name} ({DisplayName}), {spawnPoint.Position}, [{string.Join( ", ", spawnPoint.Tags )}] )" );

		// if ( forceNew || !Player.IsValid() || Player.HealthSystem.IsDead )
		// {
		// 	Player?.GameObject?.Destroy();
		// 	Player = null;

		Spawn( spawnPoint );
		// 	return;
		// }

		// Player.SetSpawnPoint( spawnPoint );
		Player.OnRespawn();
		Log.Info(" PlayerState :: Respawn FIM");
	}

	public void OnKill( DamageInfo damageInfo )
	{
		LastDamageInfo = damageInfo;
	}

	// protected void OnRespawnStateChanged( LifeState oldValue, LifeState newValue )
	// {
	// 	TimeSinceRespawnStateChanged = 0f;
	// }

	// public PlayerPawn GetLastKiller()
	// {
	// 	return GameUtils.GetPlayerFromComponent( LastDamageInfo?.Attacker );
	// }
}
