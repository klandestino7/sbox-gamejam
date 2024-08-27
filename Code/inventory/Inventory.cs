using Sandbox.Diagnostics;

namespace Gamejam;

[Category( "Inventory" )]
public class Inventory : Component
{
	[Property] public required Player Player { get; set; }

	public InventoryContainer Container { get; private set; }

	public List<ItemComponent> Items => this.Container.Items;

	public int NUM_SLOTS = 3;
	
	public int Weight { get; private set; }

	public Inventory()
	{
		Log.Info( "Initialized inventory" );
	}

	protected override void OnAwake()
	{
		base.OnAwake();

		this.Container = new InventoryContainer( this );
	}

	/*
	private void OnItemAdded( ItemComponent item )
	{
		// item.GameObject.SetupNetworking();
		item.GameObject.Network.TakeOwnership();
		item.GameObject.Parent = Player.GameObject;
		item.GameObject.Transform.Position = Player.GameObject.Transform.Position;
		item.GameObject.Transform.Rotation = Player.GameObject.Transform.Rotation;
		item.OwnedBy = Player;
	}

	private void OnItemRemoved( ItemComponent item )
	{
		item.GameObject.Parent = null;
		item.OwnedBy = null;
	}
	*/

	public void SwapItem()	
	{
	}

	public ItemComponent? GetItemAtSlot( int slotId )
	{
		return this.Container.GetItemAtSlot( slotId );
	}

	public bool TryGetValidSlotId( int slotId, out int validSlotId )
	{
		return this.Container.TryGetValidSlotId( slotId, out validSlotId );
	}

	public bool CanAddItemInfo( ItemInfo itemInfo, int slotId )
	{
		return this.Container.CanAddItemInfo( itemInfo, slotId );
	}

	public bool CanAddItem( ItemComponent item, int slotId )
	{
		return this.Container.CanAddItem( item, slotId );
	}

	public bool AddItem( ItemComponent item, int slotId = ItemComponent.INVENTORY_ANY_SLOT_ID )
	{
		Log.Warning( $"Inventory::AddItem -> Adding item with key='{ item.Info.Name }' at slot { slotId }" );

		if ( !this.TryGetValidSlotId( slotId, out int validSlotId ) )
		{
			Log.Warning( $"Inventory::AddItem -> slot is not valid!" );

			return false;
		}

		if ( !this.CanAddItem( item, validSlotId ) )
		{
			Log.Warning( $"Inventory::AddItem -> cannot add item!" );

			return false;
		}

		return this.Container.AddItem( item, validSlotId );
	}

	public ItemInfo? GetItemInfoByKey( string itemKey )
	{
		return ResourceLibrary.GetAll<ItemInfo>().FirstOrDefault( x => x.ResourceName == itemKey );
	}

	public bool AddItemWithKey( string itemKey )
	{
		var itemInfo = this.GetItemInfoByKey( itemKey );

		if ( itemInfo == null )
		{
			Log.Warning( $"couldn't find { itemKey }" );

			return false;
		}

		Log.Warning( $"Inventory::AddItemWithKey -> Adding item with key='{ itemKey }'" );

		var item = ItemComponent.Create( itemInfo );

		return this.AddItem( item);

		/*
		var prefab = PrefabLibrary
			.FindByComponent<ItemComponent>( )
			.FirstOrDefault( x => x.Name.ToLower() == itemKey.ToLower() )
			?.Prefab;

		if ( prefab == null )
		{
			var itemNames = string.Join( ',', PrefabLibrary.FindByComponent<ItemComponent>().Select( v => v.Name ) );
			Log.Warning( $"couldn't find { itemKey }" );
			Log.Warning( $"valid item names: { itemNames }" );

			return false;
		}

		int slotId = this.GetFreeSlotId();

		Log.Info($" slotId :: {slotId}");

		if ( slotId == -1 )
		{
			Log.Error( "No free slots available" );

			return false;
		}

		var gameobject = SceneUtility.GetPrefabScene( prefab ).Clone( new CloneConfig()
		{
			Transform = new(),
			Parent 	  = this.Player.GameObject,
		});

		gameobject.NetworkMode = NetworkMode.Object;
		gameobject.NetworkSpawn();

		var item = gameobject.Components.Get<ItemComponent>();

		if ( !this.AddItem( item, slotId ) )
		{
			gameobject.Destroy();

			return false;
		}

		if ( gameobject.Components.TryGet<WeaponItem>( out var weapon ) )
		{
			weapon.OwnerId = this.Player.Id;
		}


		return true;
		*/
	}

	/*
	public bool RemoveItem( ItemComponent item )
	{
		int slotId = this.GetItemSlotId( item );

		if ( slotId == -1 )
		{
			return false;
		}

		this._items[ slotId ] = null;

		this.OnItemRemoved( item );

		return true;
	}
	*/

	public void HasItemByName()
	{

	}

	public bool DropItemAtSlot( int slotId, out Pickup? outPickup )
	{
		outPickup = null;

		var item = this.Container.GetItemAtSlot( slotId );

		if ( !item.IsValid() )
		{
			Log.Warning( $"Inventory::DropItemAtSlot -> Item not found at slot { slotId }" );

			return false;
		}

		// TODO: CanDropItem?

		if ( !this.Container.RemoveItemAtSlot( slotId ) )
		{
			Log.Warning( $"Inventory::DropItemAtSlot -> Failed to remove item from slot { slotId } " );

			return false;
		}

		var aimRay = Player.Local.CameraController.AimRay;

		outPickup = Pickup.Create( item.Info, aimRay.Position + aimRay.Forward * 128.0f, item.Amount );

		Log.Warning( $"Inventory::DropItemAtSlot -> Item at slot { slotId } was dropped!" );

		return true;
	}

	/*
	public WeaponItem? AddWeapon( WeaponInfo weaponInfo )
	{
		if ( !weaponInfo.MainPrefab.IsValid() )
		{
			Log.Error( "WeaponInfo.MainPrefab is not valid" );

			return null;
		}

		var gameObject = weaponInfo.MainPrefab.Clone( new CloneConfig()
		{
			Transform = new(),
			Parent 	  = this.Player.GameObject,
		});

		gameObject.NetworkSpawn( Player.Network.OwnerConnection );

		WeaponItem weapon = gameObject.Components.Get<WeaponItem>( FindMode.EverythingInSelfAndDescendants );
		weapon.OwnerId = Player.Id;

		this.AddItem( weapon, -2 );

		return weapon;
	}
	*/

	[ConCmd( "giveitem" )]
	public static void OnCommandGiveItem( string itemKey )
	{
		Player player = Player.Local;

		if ( !player.IsValid() )
		{
			return;
		}

		player.Inventory.AddItemWithKey( itemKey );
	}

	[ConCmd( "giveweapon" )]
	public static void OnGiveWeaponCommand()
	{
		WeaponInfo? weaponInfo = Sandbox.ResourceLibrary.Get<WeaponInfo>( "weapons/flashlight/flashlight.weapon" );

		if ( weaponInfo == null )
		{
			Log.Error( "WeaponInfo not found" );

			return;
		}

		// Player.Local.Inventory.AddWeapon( weaponInfo );
	}

	[ConCmd( "dropitem" )]
	public static void OnDropItemCommand( int slotId )
	{
		Player.Local.Inventory.DropItemAtSlot( slotId, out _ );
	}
}
