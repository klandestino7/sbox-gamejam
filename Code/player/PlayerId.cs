namespace Gamejam;

/// <summary>
/// Grants a unique Id to each player, generated on host and synced.
/// When a player disconnects that Id is returned to the pool and can be issued to new players. Issues an Id globally and per-team.
/// </summary>
public class PlayerId : Component
{
	[RequireComponent] PlayerState PlayerState { get; set; }

	private struct PlayerIdGenerator
	{
		private Stack<int> freeIds;
		private int maxId;

		public PlayerIdGenerator()
		{
			freeIds = new Stack<int>();
			maxId = 0;
		}

		public int Get()
		{
			int id;
			if ( freeIds.TryPop( out id ) )
			{
				return id;
			}

			id = maxId;
			maxId++;
			return id;
		}

		public void Free(int id)
		{
			if (id != -1)
				freeIds.Push( id );
		}
	}

	private static PlayerIdGenerator uniqueGenerator;
	private static PlayerIdGenerator[] teamGenerator;

	/// <summary>
	/// Unique Id of this player in the game. New players will occupy vacant ids.
	/// </summary>
	[HostSync] public int UniqueId { get; private set; } = -1;

	/// <summary>
	/// Unique Id of this player within their team. New players will occupy vacant ids.
	/// </summary>
	[HostSync] public int TeamUniqueId { get; private set; } = -1;

	protected override void OnAwake()
	{
		if ( !Networking.IsHost )
			return;
		
		UniqueId = uniqueGenerator.Get();
	}

	public void Free()
	{
		if ( !Networking.IsHost )
			return;

		uniqueGenerator.Free( UniqueId );
	}

	public static void Init()
	{
		uniqueGenerator = new();
	}
}
