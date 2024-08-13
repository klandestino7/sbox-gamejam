namespace Gamejam;

[GameResource( "ItemInfo", "item", "yes", IconBgColor = "#5877E0", Icon = "track_changes"  )]
public class ItemInfo : Sandbox.GameResource
{
	public static HashSet<ItemInfo> All { get; set; } = new();

	[Category( "Base" )]
	public String Name { get; set; } = "Custom Item";

	[Category( "Prefabs" )]
	public GameObject? MainPrefab { get; set; }

	protected override void PostLoad()
	{
		Log.Info( $"Loaded ItemInfo {Name}" );

		base.PostLoad();

		if ( All.Contains( this ) )
		{
			Log.Warning( "Tried to add two of the same ItemInfo (?)" );
			return;
		}

		All.Add( this );
	}
}
