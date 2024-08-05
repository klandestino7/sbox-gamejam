using Gamejam;
using Sandbox;
namespace Sandbox.Inventory;

public class ItemComponent : Component
{
	[Property] public string? Name { get; set; }
	[Property] public string? Description { get; set; }
	[Property] public string? Icon { get; set; }
	[Property, Sync] public string? Weight { get; set; }
	[Sync] public string? Prefab { get; private set; }

	public Player? OwnedBy { get; set; }

	protected override void OnStart()
	{

	}

	protected override void OnAwake()
	{
		base.OnAwake();
		Prefab = GameObject.PrefabInstanceSource;
	}

	protected override void OnDestroy()
	{
		if ( IsProxy || !Game.IsPlaying )
			return;
	}
}
