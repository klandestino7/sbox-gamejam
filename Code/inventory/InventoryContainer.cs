using Sandbox.Diagnostics;

namespace Gamejam;



public struct InventoryContainer
{
	public List<ItemComponent> Items { get; set; } = new();
	public int NumSlots { get; set; } = 3;

	public Inventory Inventory { get; set; }

	public InventoryContainer( Inventory inventory )
	{
		this.Inventory = inventory;

		for ( int i = 0; i < this.NumSlots; i++ )
		{
			Items.Add( null );
		}
	}

	public int GetItemSlotId( ItemComponent? item )
	{
#pragma warning disable CS8604 // Possible null reference argument.
		return this.Items.IndexOf( item );
#pragma warning restore CS8604 // Possible null reference argument.
	}

	public ItemComponent? GetItemAtSlot( int slotId )
	{
		return this.Items.ElementAtOrDefault( slotId );
	}

	public int GetFreeSlotId()
	{
		for ( int slotId = 0; slotId < this.NumSlots; slotId++ )
		{
			if ( this.Items[ slotId ] == null )
			{
				return slotId;
			} 
		}

		return ItemComponent.INVENTORY_INVALID_SLOT_ID;
	}

	public bool TryGetValidSlotId( int slotId, out int validSlotId )
	{
		Log.Info( $"InventoryContainer::TryGetValidSlotId -> slotId={ slotId }" );

		if ( slotId == ItemComponent.INVENTORY_ANY_SLOT_ID )
		{
			slotId = this.GetFreeSlotId();
		}

		Log.Info( $"InventoryContainer::TryGetValidSlotId -> slotId={ slotId } isValid={ this.IsSlotValid( slotId ) }" );

		if ( !this.IsSlotValid( slotId ) )
		{
			validSlotId = ItemComponent.INVENTORY_INVALID_SLOT_ID;

			return false;
		}

		validSlotId = slotId;

		return true;
	}

	public bool CanAddItemInfo( ItemInfo itemInfo, int slotId )
	{
		this.AssetIsSlotValid( slotId );

		if ( this.Items[ slotId ] != null )
		{
			return false;
		}

		return true;
	}

	public bool CanAddItem( ItemComponent item, int slotId )
	{
		if ( !this.CanAddItemInfo( item.Info, slotId ) )
		{
			return false;
		}

		/*
		if ( item?.OwnedBy == this.Player )
		{
			return false;
		}
		*/

		return true;
	}

	public bool HasItemAtSlot( int slotId )
	{
		return this.Items[ slotId ] != null;
	}

	public bool IsSlotValid( int slotId )
	{
		return slotId > ItemComponent.INVENTORY_MAX_INVALID_SLOT_ID;
	}

	public void AssetIsSlotValid( int slotId )
	{
		Assert.True( this.IsSlotValid( slotId ), "Invalid slotId" );
	}

	public bool AddItem( ItemComponent item, int slotId )
	{
		this.AssetIsSlotValid( slotId );

		this.Items[ slotId ] = item;

		Log.Info( $"InventoryContainer::AddItem -> Item added with key='{ item.Name }' at slotId { slotId }" );

		return true;
	}

	public bool RemoveItemAtSlot( int slotId )
	{
		this.AssetIsSlotValid( slotId );

		if ( !this.HasItemAtSlot( slotId ) )
		{
			return false;
		}

		this.Items[ slotId ] = null;

		return true;
	}
}
