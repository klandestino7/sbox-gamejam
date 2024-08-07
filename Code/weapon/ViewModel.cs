namespace Gamejam;

public class ViewModel : Component
{
	[Property] public SkinnedModelRenderer ModelRenderer { get; set; }
	[Property] public bool UseSprintAnimation { get; set; }
	
	/// <summary>
	/// Looks up the tree to find the player controller.
	/// </summary>
	// private PlayerController PlayerController => Weapon.Components.GetInAncestors<PlayerController>();
	private CameraComponent Camera { get; set; }
	private Weapon Weapon { get; set; }

	public void SetWeapon( Weapon weapon )
	{
		Weapon = weapon;
	}

	public void SetCamera( CameraComponent camera )
	{
		Camera = camera;
	}
}
