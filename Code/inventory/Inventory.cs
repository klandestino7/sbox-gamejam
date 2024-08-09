namespace Gamejam;

public class Inventory : Component
{
	[Property] public Player Player { get; set; }

	public int NUM_SLOTS = 3;
	
	public int Weight { get; private set; }

	private List<ItemComponent?> _items;

	public IReadOnlyList<ItemComponent?> Items => this._items;



	public Inventory()
	{
		Log.Info( "Initialized inventory" );

		_items = new List<ItemComponent?>( new ItemComponent[NUM_SLOTS] );
	}

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

	public void SwapItem()	
	{

	}

	public ItemComponent? GetItemAtSlot( int slotId )
	{
		return this._items.ElementAtOrDefault( slotId );
	}

	public int GetItemSlotId( ItemComponent? item )
	{
#pragma warning disable CS8604 // Possible null reference argument.
		return this._items.IndexOf( item );
#pragma warning restore CS8604 // Possible null reference argument.
	}

	public int GetFreeSlotId()
	{
		return this.GetItemSlotId( null );
	}

	public bool CanAddItem( ItemComponent item, int slotId )
	{
		if ( this._items[ slotId ] != null )
		{
			return true;
		}

		if ( item?.OwnedBy == this.Player )
		{
			return false;
		}

		return true;
	}

	public bool AddItem( ItemComponent item, int slotId )
	{
		if ( !this.CanAddItem( item, slotId ) )
		{
			return false;
		}

		this._items[ slotId ] = item;

		Log.Info( $"AddItem :: Name='{ item.Name }'" );

		return true;
	}

	public bool AddItemByKey( string itemKey )
	{
		var prefab = PrefabLibrary
			.FindByComponent<ItemComponent>( )
			.Where( p =>
			{
				Log.Info( $"itemname {  p.GetComponent<ItemComponent>().Get<string>("Name").ToLower() } other= { itemKey.ToLower() }" );

				return p.GetComponent<ItemComponent>().Get<string>("Name").ToLower() == itemKey.ToLower();
			}
			).FirstOrDefault()
			?.Prefab;

		if ( prefab == null )
		{
			Log.Error( $"Item not found with key equals to '{ itemKey }'" );
		}

		int slotId = this.GetFreeSlotId();

		Log.Info($" slotId :: {slotId}");

		if ( slotId == -1 )
		{
			Log.Error( "No free slots available" );

			return false;
		}

		var gameobject = SceneUtility.GetPrefabScene( prefab ).Clone();

		gameobject.NetworkMode = NetworkMode.Object;
		gameobject.NetworkSpawn();

		if ( !this.AddItem( gameobject.Components.Get<ItemComponent>(), slotId ) )
		{
			gameobject.Destroy();

			return false;
		}

		return true;
	}

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

	public void HasItemByName()
	{

	}

	public bool DropItem( ItemComponent item )
	{
		if ( !this.RemoveItem( item ) )
		{
			return false;
		}

		// TODO: Add a viewray property to Player
		// SceneTrace trace = Scene.Trace.FromTo( this.Player.EyePos. )

		return true;
	}

	public Weapon? AddWeapon( WeaponInfo weaponInfo )
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

		Weapon weapon = gameObject.Components.Get<Weapon>( FindMode.EverythingInSelfAndDescendants );
		weapon.OwnerId = Player.Id;

		return weapon;
	}

	[ConCmd( "giveitem" )]
	public static void OnCommandGiveItem( string itemKey )
	{
		Player player = Player.Local;

		Log.Info("giveitem :: ");

		Log.Info($"player :: {player}");

		if ( player == null )
		{
			return;
		}

		player.Inventory.AddItemByKey( itemKey );
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

		Player.Local.Inventory.AddWeapon( weaponInfo );
	}
}
