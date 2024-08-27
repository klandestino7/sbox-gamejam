
namespace Gamejam;

public class ItemComponent : IValid
{
	public const int INVENTORY_ANY_SLOT_ID = -2;

	public const int INVENTORY_INVALID_SLOT_ID = -1;

	public const int INVENTORY_MAX_INVALID_SLOT_ID = INVENTORY_INVALID_SLOT_ID;

	public required ItemInfo Info;

	public string Name => Info.Name;

	public int Amount = 1;

	public Inventory? Parent;

	public static ItemComponent Create( ItemInfo info, int amount = 1 )
	{
		return new ItemComponent { Info = info, Amount = amount };
	}

	public bool IsValid => this.Info != null && ( this.Parent?.IsValid() ?? false );
}
