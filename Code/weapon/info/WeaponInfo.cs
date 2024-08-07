namespace Gamejam;

[GameResource( "Weapon Info", "weapon", "yes", IconBgColor = "#5877E0", Icon = "track_changes"  )]
public class WeaponInfo : Sandbox.GameResource
{
	[Category( "Base" )]
	public String Name { get; set; } = "Custom Weapon";

	[Category( "Prefabs" )]
	public GameObject? Prefab { get; set; }

	[Category( "Prefabs" )]
	public GameObject? ViewModelPrefab { get; set; }
}
