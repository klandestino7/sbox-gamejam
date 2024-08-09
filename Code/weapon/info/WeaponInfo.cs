namespace Gamejam;

[GameResource( "WeaponInfo", "weapon", "yes", IconBgColor = "#5877E0", Icon = "track_changes"  )]
public class WeaponInfo : Sandbox.GameResource
{
	public static HashSet<WeaponInfo> All { get; set; } = new();

	[Category( "Base" )]
	public String Name { get; set; } = "Custom Weapon";

	[Category( "Prefabs" )]
	public GameObject? MainPrefab { get; set; }

	[Category( "Prefabs" )]
	public GameObject? ViewModelPrefab { get; set; }

	protected override void PostLoad()
	{
		Log.Info( $"Loaded WeaponInfo {Name}" );

		base.PostLoad();

		if ( All.Contains( this ) )
		{
			Log.Warning( "Tried to add two of the same WeaponInfo (?)" );
			return;
		}

		All.Add( this );
	}
}
