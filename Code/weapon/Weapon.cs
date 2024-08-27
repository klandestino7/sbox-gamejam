namespace Gamejam;

public enum eAttachPoint
{
	LeftHand,
	RightHand,
}

public enum eHoldType
{
	Rifle,
}

public class WeaponItem : Component
{
	public virtual string Title { get; set; } = "Generic Item";

	// [Property] public SkinnedModelRenderer? ModelRenderer { get; set; }

	[Property] protected eAttachPoint AttachPoint { get; set; } = eAttachPoint.RightHand;

	[Property] protected eHoldType HoldType { get; set; } = eHoldType.Rifle;

	private Player? _owner;

	public Player? Owner
	{
		get => _owner ??= Scene.Directory.FindComponentByGuid( OwnerId ) as Player;
	}

	[HostSync] public Guid OwnerId { get; set; }

	[Sync] public bool IsDeployed { get; private set; }

	public virtual void PrimaryAttack()
	{
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
	}

	protected override void OnStart()
	{
		if ( !this.IsDeployed )
		{
			this.OnDeployed();
		}
		else
		{

		}
	}

	protected void UpdateRenderMode()
	{
		bool enabled = this.Owner.IsValid() && !this.Owner.IsPossessed && this.IsDeployed;
	}

	protected virtual void OnDeployed()
	{
		this.UpdateRenderMode();
	}
}
