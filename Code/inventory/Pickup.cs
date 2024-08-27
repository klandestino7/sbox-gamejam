namespace Gamejam;

public class Pickup : Component, Component.ITriggerListener
{
	[Property] public ItemInfo ItemInfo { get; set; }

	[Sync] public int Amount { get; set; } = 1;

	[Property] SphereCollider SphereCollider { get; set; }

	[Property] SkinnedModelRenderer ModelRenderer { get; set; }

	public static Pickup Create( ItemInfo itemInfo, Vector3 position, int quantity = 1 )
	{
		GameObject obj = new GameObject();
		obj.Name = "Pickup";
		obj.Transform.Position = position;

		SphereCollider sphereCollider = obj.Components.Create<SphereCollider>();
		sphereCollider.Radius = 16;

		Pickup pickup = obj.Components.Create<Pickup>();
		pickup.ItemInfo = itemInfo;
		pickup.Amount = quantity;

		var modelRenderer = obj.Components.Create<SkinnedModelRenderer>();
		modelRenderer.Model = itemInfo.WorldModel;
		// mdl.Transform.Local = itemInfo.WorldModelOffset;

		obj.NetworkSpawn();

		return pickup;
	}

	void ITriggerListener.OnTriggerEnter( Sandbox.Collider other )
	{
		if ( this.Amount <= 0 )
		{
			return;
		}

		if ( !Sandbox.Networking.IsHost )
		{
			return;
		}

		if ( other.GameObject.Root.Components.Get<Player>() is not { IsValid: true } player )
		{
			return;
		}

		// TODO: Don't allow collecting pickup too fast

		using ( Rpc.FilterInclude( player.Network.OwnerConnection ) )
		{
			this.OnCollect( player );
		}
	}

	[ Broadcast( NetPermission.HostOnly ) ]
	public void OnCollect( Player player )
	{
		var item = ItemComponent.Create( this.ItemInfo, this.Amount );

		if ( !player.Inventory.AddItem( item ) )
		{
			return;
		}

		this.DestroyOnAuthority();
	}

	[ Authority ]
	public void DestroyOnAuthority()
	{
		this.GameObject.Destroy();
	}
}
