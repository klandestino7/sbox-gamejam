namespace Gamejam;

partial class Player
{
	public static IReadOnlyList<Player> All => _internalPlayers;
	public static List<Player> _internalPlayers = new List<Player>();

	public static Player Local { get; set; }

	/// <summary>
	/// Are we possessing this pawn right now? (Clientside)
	/// </summary>
	public bool IsPossessed => Local == this;

	private Guid _guid;

	[HostSync]
	public Guid ConnectionID
	{
		get => _guid;
		set
		{
			_guid = value;
			Connection = Connection.Find( _guid );
			
			if ( _guid == Connection.Local.Id )
				Local = this;

			if ( !_internalPlayers.Contains( this ) )
				_internalPlayers.Add( this );
		}
	}

	public Connection Connection { get; private set; }

	public ulong SteamID => Connection.SteamId;
	public string Name => Connection.DisplayName;

	public void SetupConnection( Connection connection )
	{
		ConnectionID = connection.Id;
		GameObject.Name = $"{Name} / {SteamID}";
	}

	/// <summary>
	/// Possess the pawn.
	/// </summary>
	public void Possess() => Possess(this);
	public static void Possess( Player player )
	{
		if ( player.IsPossessed )
			return;

		DePossess( Local );
		Local = player;
		player?.OnPossess();

		// Valid and we own it?
		if ( player.IsValid() && !player.IsProxy )
		{
			// player.SteamID = Connection.Local.SteamId;
		}

		PlayerState.OnPossess( player );
	}

	/// <summary>
	/// De possesses the pawn.
	/// </summary>
	public void DePossess() => DePossess( this );
	public static void DePossess( Player player )
	{ 
		bool wasPossessed = player.IsValid() && player.IsPossessed;
		Local = null;

		if ( wasPossessed )
		{
			player?.OnDePossess();

			// Valid and we own it?
			if ( player.IsValid() && !player.IsProxy )
			{
				// player.SteamID = 0;
			}
		}
	}

	// public virtual void OnPossess() { }
	// public virtual void OnDePossess() { }

	public static Player GetByID( Guid id )
		=> _internalPlayers.FirstOrDefault( x => x.ConnectionID == id );
}
