using Sandbox;
namespace Gamejam;

public class Inventory : Component
{
    [Property] public Player Player { get; set; }
    public const int MAX_SLOTS_INV = 3;
    public int Weight { get; private set; }

    public IReadOnlyList<ItemInventory> Items => _items;
	private readonly List<ItemInventory> _items;

    public Inventory()
    {
		_items = new List<ItemInventory>( new ItemInventory[3] );
    }

    public void SwapItem() 
    {
        
    }

    public void AddItem()
    {

    }

    public void RemoveItem()
    {

    }

    public void HasItemByName()
    {

    }

    public void DropItem()
    {

    }
}