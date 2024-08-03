

using Sandbox.Network;
using System.Threading.Tasks;
using Sandbox.Events;
using System.Threading.Channels;
using Sandbox.Diagnostics;

namespace Gamejam;

public sealed class GameModeManager : SingletonComponent<GameModeManager>, Component.INetworkListener
{

    
	[Broadcast( NetPermission.HostOnly )]
    public static void StartSession( PlayerState playerState)
	{
		// Player.Local.BlackScreen( 0f, 4f, 1f );
		// UI.Hud.Instance.Panel.PlaySound( "car_intro" );
		// await GameTask.DelayRealtimeSeconds( 2f );

        CreateSession( playerState );
	}

    public static void CreateSession( PlayerState playerState )
    {
        var gameMode = Game.ActiveScene.GetAllComponents<GameModeManager>().First();
		if ( gameMode == null ) return;


		// playerState.HostInit();
		// playerState.ClientInit();

        // Player.Local.PlayerState.Respawn( true );
    }

	[Broadcast( NetPermission.HostOnly )]
    public static void EndSession()
    {
        
    }
}