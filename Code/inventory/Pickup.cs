namespace Gamejam;

public class Pickup : Component, IContextActionProvider
{
	[Property] public ItemInfo ItemInfo { get; set; }

	[Sync] public int Amount { get; set; } = 1;

	public Color GlowColor => Color.Yellow;
	public bool AlwaysGlow => false;
	public float InteractionRange => 100f;
	public Vector3 Position => Transform.Position;

	private ContextAction PickupAction { get; set; }

	public static Pickup Create( ItemInfo itemInfo, Vector3 position, int quantity = 1 )
	{
		GameObject obj = new GameObject();
		obj.Name = "Pickup";
		obj.Transform.Position = position;
		obj.Tags.Add( "interaction" );

		Pickup pickup = obj.Components.Create<Pickup>();
		pickup.ItemInfo = itemInfo;
		pickup.Amount = quantity;

		var modelRenderer = obj.Components.Create<ModelRenderer>();
		modelRenderer.Model = itemInfo.WorldModel;

		obj.NetworkSpawn();

		return pickup;
	}

	protected override void OnStart()
	{
		PickupAction = new ContextAction( "pickup", $"Pick Up", "ui/actions/open_door.png" );
		Tags.Add( "interaction" );
	}

	public string GetContextName() => ItemInfo?.Name ?? "Item";

	public ContextAction GetPrimaryAction( Player player ) => PickupAction;

	public IEnumerable<ContextAction> GetSecondaryActions( Player player )
	{
		yield break;
	}

	public void OnContextAction( Player player, ContextAction action )
	{
		if ( action != PickupAction || Amount <= 0 )
			return;

		using ( Rpc.FilterInclude( player.Network.OwnerConnection ) )
		{
			OnCollect( player );
		}
	}

	[Broadcast( NetPermission.HostOnly )]
	public void OnCollect( Player player )
	{
		var item = ItemComponent.Create( ItemInfo, Amount );

		if ( !player.Inventory.AddItem( item ) )
			return;

		DestroyOnAuthority();
	}

	[Authority]
	public void DestroyOnAuthority()
	{
		GameObject.Destroy();
	}
}
