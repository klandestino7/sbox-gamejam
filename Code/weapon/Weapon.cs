namespace Gamejam;

public enum eAttachPoint
{
	LeftHand,
	RightHand,
}

public enum eHoldType
{
	Rifle,
}

public class Weapon : Component, IContextActionProvider
{
	public Color GlowColor => Color.Green;
	public bool AlwaysGlow => true;
	public float InteractionRange => 100f;
	public Vector3 Position { get; set; }
    
	private ContextAction GetWeapon { get; set; }
	public virtual string Title { get; set; } = "Generic Item";
    
	[Property] public GameObject ViewModelPrefab { get; set; }

	// [Property] public SkinnedModelRenderer BodyRenderer { get; set; }

	[Property] protected eAttachPoint AttachPoint { get; set; } = eAttachPoint.RightHand;

	[Property] protected eHoldType HoldType { get; set; } = eHoldType.Rifle;

	private SkinnedModelRenderer ModelRenderer { get; set; }

	private ViewModel ViewModel { get; set; }

	public bool HasViewModel => ViewModel.IsValid();

	public virtual void PrimaryAttack()
	{
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
	}

	protected override void OnStart()
	{
		GetWeapon = new ContextAction( "weapon.get", "Pickup", "ui/actions/pickup.png" );
		Position = Transform.Position;
		Tags.Add("interaction");

		OnDeployed();
		// base.OnStart();
	}

	protected virtual void OnDeployed()
	{
		this.ModelRenderer.Enabled = !this.HasViewModel;

		if ( !this.IsProxy )
		{
			this.CreateViewModel();
		}
	}

	protected void CreateViewModel()
	{
		if ( !this.ViewModelPrefab.IsValid() )
		{
			return;
		}

		Player player = Components.GetInAncestors<Player>();

		GameObject gameobject = this.ViewModelPrefab.Clone();
		gameobject.Flags |= GameObjectFlags.NotNetworked;
		gameobject.SetParent( player.GameObject );
		
		this.ViewModel = gameobject.Components.Get<ViewModel>();
		this.ViewModel.SetWeapon( this );
		this.ViewModel.SetCamera( player.ViewModelCamera );

		this.ModelRenderer.Enabled = false;
	}

	public string GetContextName()
	{
		return Title;
	}

	public IEnumerable<ContextAction> GetSecondaryActions( Player player )
	{
		yield break;
	}

	public ContextAction GetPrimaryAction( Player player )
	{
		return GetWeapon;
	}

	public virtual void OnContextAction( Player player, ContextAction action )
	{
		if ( action == GetWeapon )
		{
			Log.Info(" Lógica de Pegar Arma ");
		}
	}

}
