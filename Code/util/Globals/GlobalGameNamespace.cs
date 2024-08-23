namespace Gamejam;

/// <summary>
/// It's nice to be able to access a global anywhere.
/// </summary>
public static class GlobalGameNamespace
{
	/// <summary>
	/// Fetch a global.
	/// </summary>
	public static T GetGlobal<T>() where T : GlobalComponent
	{
		return Sandbox.Game.ActiveScene.GetSystem<GlobalSystem>().Get<T>();
	}
}
