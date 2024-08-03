using Gamejam;
using Sandbox;
namespace Sandbox.inventory;

public class Inventory : Component
{
	[Property] public Player Player { get; set; }

	public const int MAX_NUM_INVENTORY_ITEMS = 3;
	
	public int Weight { get; private set; }

	private List<ItemComponent> Items;

	public Inventory()
	{
		Items = new List<ItemComponent>( MAX_NUM_INVENTORY_ITEMS );
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

	public int GetItemSlotId( ItemComponent item )
	{
		return this.Items.IndexOf( item );
	}

	public bool CanAddItem( ItemComponent item )
	{
		int freeSlotId = this.GetItemSlotId( null );

		if ( freeSlotId == -1 )
		{
			return false;
		}

		if ( item?.OwnedBy == this.Player )
		{
			return false;
		}

		return true;
	}

	public bool AddItem( ItemComponent item )
	{
		if ( !this.CanAddItem( item ) )
		{
			return false;
		}

		this.Items.Add( item );

		return true;
	}

	public bool AddItemByKey( string itemKey )
	{
		var prefab = PrefabLibrary
			.FindByComponent<ItemComponent>()
			.FirstOrDefault( p => p.Name.ToLower() == itemKey.ToLower() )
			?.Prefab;

		if ( prefab == null )
		{
			Log.Error( $"Item not found with key equals to '{ itemKey }'" );
		}

		var gameobject = SceneUtility.GetPrefabScene( prefab ).Clone();
		gameobject.NetworkMode = NetworkMode.Object;
		gameobject.NetworkSpawn();

		if ( !this.AddItem( gameobject.Components.Get<ItemComponent>() ) )
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

		this.Items.RemoveAt( slotId );

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

	[ConCmd( "giveitem" )]
	public static void OnCommandGiveItem( string itemKey )
	{
		Player player = Player.Local;

		if ( player == null )
		{
			return;
		}

		player.Inventory.AddItemByKey( itemKey );
	}
}
