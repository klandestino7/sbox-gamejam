namespace Gamejam;

[GameResource( "WeaponInfo", "weapon", "yes", IconBgColor = "#5877E0", Icon = "track_changes"  )]
public class WeaponInfo : ItemInfo
{
	[Category( "Prefabs" )]
	public GameObject? MainPrefab { get; set; }

	[Category( "Prefabs" )]
	public GameObject? ViewModelPrefab { get; set; }
}
