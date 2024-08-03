using Sandbox.Diagnostics;
using Sandbox.Events;

namespace Gamejam;

public partial class PlayerState : Component
{
	/// <summary>
	/// The player we're currently in the view of (clientside).
	/// Usually the local player, apart from when spectating etc.
	/// </summary>
	public static PlayerState Viewer { get; private set; }

	/// <summary>
	/// Our local player on this client.
	/// </summary>
	public static PlayerState Local { get; private set; }

	// --

	/// <summary>
	/// Who owns this player state?
	/// </summary>
	[HostSync, Property] public ulong SteamId { get; set; }

	/// <summary>
	/// The player's name, which might have to persist if they leave
	/// </summary>
	[HostSync] private string SteamName { get; set; }

	/// <summary>
	/// The connection of this player
	/// </summary>
	public Connection Connection => Network.OwnerConnection;
	public bool IsConnected => Connection is not null && (Connection.IsActive || Connection.IsHost); //smh

	private string name => SteamName ?? "";
	/// <summary>
	/// Name of this player
	/// </summary>
	public string DisplayName => $"{name}{(!IsConnected ? " (Disconnected)" : "")}";

	/// <summary>
	/// Unique Ids of this player
	/// </summary>
	[RequireComponent] public PlayerId PlayerId { get; private set; }

	/// <summary>
	/// Are we in the view of this player (clientside)
	/// </summary>
	public bool IsViewer => Viewer == this;

	/// <summary>
	/// Is this the local player for this client
	/// </summary>
	public bool IsLocalPlayer => !IsProxy && Connection == Connection.Local;

	/// <summary>
	/// The main PlayerPawn of this player if one exists, will not change when the player possesses gadgets etc. (synced)
	/// </summary>
	[HostSync] public Player Player { get; set; }

	public void HostInit()
	{
		// // on join, spawn right now if we can
		// RespawnState = RespawnState.Immediate;
		
		SteamId = Connection.SteamId;
		SteamName = Connection.DisplayName;
	}

	[Authority]
	public void ClientInit()
	{
		Local = this;
	}

	public void Kick()
	{
		if ( Player.IsValid() )
		{
			Player.GameObject.Destroy();
		}

		GameObject.Destroy();
		// todo: actually kick em
	}

	public static void OnPossess( Player player )
	{
		// called from Pawn when one is newly possessed, update Local and Viewer, invoke RPCs for observers

		Local.Player = player;

		if ( player.Network.Active )
		{
			Local.OnNetPossessed();
		}

		if ( !player.PlayerState.IsValid() )
		{
			Log.Warning( $"Attempted to possess pawn, but pawn '{player.Name}' has no attached PlayerState!" );
		}

		Viewer = player.PlayerState;
	}

	// sync to other clients what this player is currently possessing
	// Sol: when we track observers we could drop this with an Rpc.FilterInclude?
	[Broadcast]
	private void OnNetPossessed()
	{
		if ( IsViewer && IsProxy )
		{
			Possess();
		}
	}

	public void Possess()
	{
		// A remote player is possessing this player (spectating)
		// So enter the latest known pawn this player has possessed
		Player.Possess();
	}

}
