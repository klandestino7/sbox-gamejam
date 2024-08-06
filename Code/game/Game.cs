using Sandbox.Diagnostics;

namespace Gamejam;

public enum eGameLevel
{
	Menu,
	Hideout,
	Level0,
	Level1,
	Dev
}

public enum eGameSessionState
{
	Idle,

	/// <summary>
	/// The game is currently in the lobby.
	/// </summary>
	Init,

	/// <summary>
	/// The game is currently in progress.
	/// </summary>
	LoadLevel,
}

public enum eGameState
{
	Playing,
	Respawning,
}


public class Game : SingletonComponent<Game>
{
	private SceneFile SceneFileMenu = ResourceLibrary.Get<SceneFile>( "scenes/MainMenu.scene" );
	private SceneFile SceneFileHideout = ResourceLibrary.Get<SceneFile>( "scenes/hideout.scene" );
	private SceneFile SceneFileDev = ResourceLibrary.Get<SceneFile>( "scenes/minimal.scene" );
	private SceneFile SceneFileLevel0 = ResourceLibrary.Get<SceneFile>( "scenes/shopping.scene" );

	public eGameSessionState SessionState = eGameSessionState.Init;
	public eGameLevel Level = eGameLevel.Menu;
	public eGameState State = eGameState.Playing;

	public bool IsPaused
	{
		get => Sandbox.Game.ActiveScene.TimeScale <= 0.0f;
		set => Sandbox.Game.ActiveScene.TimeScale = value ? 0.0f : 1.0f;
	}

	protected override void OnStart()
	{
		Log.Info( "Game started" );

		switch (Sandbox.Game.ActiveScene.Title)
		{
			case "mainmenu":
				this.Level = eGameLevel.Menu;
				break;
			case "hideout":
				this.Level = eGameLevel.Hideout;
				break;
			case "shopping":
				this.Level = eGameLevel.Level0;
				break;
			case "minimal":
				this.Level = eGameLevel.Dev;
				break;
			default:
				throw new Exception( $"Invalid level SceneFile Title \"{ Sandbox.Game.ActiveScene.Title }\"" );
		}

		Log.Info( $"Game started with level { this.Level }" );

		base.OnStart();
	}

	public void LoadScene( SceneFile sceneFile )
	{
        Sandbox.Game.ActiveScene.Load( sceneFile );
	}

	public void InitLevel( eGameLevel level )
	{
		Assert.Equals( Sandbox.Networking.IsHost, true );

		this.SessionState = eGameSessionState.LoadLevel;

		Log.Info( $"Loading level { level }" );

		switch( level )
		{
			case eGameLevel.Menu:
				this.LoadScene( this.SceneFileMenu );
				break;
			case eGameLevel.Hideout:
				this.LoadScene( this.SceneFileHideout );
				break;
			case eGameLevel.Level0:
				this.LoadScene( this.SceneFileLevel0 );
				break;
			case eGameLevel.Dev:
				this.LoadScene( this.SceneFileDev );
				break;
			default:
				throw new Exception( "Invalid level" );
		}

		this.SessionState = eGameSessionState.Idle;
	}

	public void ShutdownLevel()
	{
		this.LoadScene( this.SceneFileMenu );

		this.SessionState = eGameSessionState.Idle;
	}

	public void HostLobby()
	{
		Assert.Equals( this.Level, eGameLevel.Menu );

		this.InitLevel( eGameLevel.Level0 );
	}
}
