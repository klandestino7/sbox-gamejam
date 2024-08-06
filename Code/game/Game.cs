using Sandbox.Diagnostics;

namespace Gamejam;

public enum eGameLevel
{
	Menu,
	Level0,
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

	private SceneFile SceneFileLevel0 = ResourceLibrary.Get<SceneFile>( "scenes/minimal.scene" );

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
			case "minimal":
				this.Level = eGameLevel.Level0;
				break;
			default:
				throw new Exception( $"Invalid level SceneFile Title { Sandbox.Game.ActiveScene.Title }" );
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
		this.SessionState = eGameSessionState.LoadLevel;

		Log.Info( $"Loading level { level }" );

		switch( level )
		{
			case eGameLevel.Menu:
				this.LoadScene( this.SceneFileMenu );
				break;
			case eGameLevel.Level0:
				this.LoadScene( this.SceneFileLevel0 );
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
}
