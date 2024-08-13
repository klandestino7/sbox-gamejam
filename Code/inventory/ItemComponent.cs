
namespace Gamejam;

public class ItemComponent : Component, IContextActionProvider
{
	public String Name { get; set; } = "Custom Item";

	[Property] protected ItemInfo? Info { get; set; }

	public Player? OwnedBy { get; set; }

	public Color GlowColor => Color.Green;
	public bool AlwaysGlow => true;
	public float InteractionRange => 100f;
	public Vector3 Position { get; set; }

	private ContextAction? ActionCollect { get; set; }

	protected override void OnStart()
	{
		ActionCollect = new ContextAction( "weapon.get", "Pickup", "ui/actions/pickup.png" );
	}

	protected override void OnAwake()
	{
		base.OnAwake();

		// Prefab = GameObject.PrefabInstanceSource;

		// Prefab.Components<>();
	}
	protected override void OnDestroy()
	{
		if ( IsProxy || !Sandbox.Game.IsPlaying )
			return;
	}

	protected virtual bool CanCollect()
	{
		return true;
	}

	protected virtual void OnCollect()
	{
	}

	protected virtual void Collect()
	{
		if ( !CanCollect() )
		{
			return;
		}

		OnCollect();
	}

	public virtual ItemInfo? GetInfo()
	{
		return this.Info;
	}

	IEnumerable<ContextAction> IContextActionProvider.GetSecondaryActions( Player player )
	{
		yield break;
	}

	ContextAction IContextActionProvider.GetPrimaryAction( Player player )
	{
		return ActionCollect!;
	}

	void IContextActionProvider.OnContextAction( Player player, ContextAction action )
	{
		if ( action == ActionCollect )
		{
			this.Collect();
		}
	}

	string IContextActionProvider.GetContextName()
	{
		return this.Info.Name!;
	}
}
