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
	[Property, Group( "Resources" )] public WeaponInfo Info { get; set; }

	public Color GlowColor => Color.Green;
	public bool AlwaysGlow => true;
	public float InteractionRange => 100f;
	public Vector3 Position { get; set; }
    
	private ContextAction GetWeapon { get; set; }
	public virtual string Title { get; set; } = "Generic Item";

	[Property] public SkinnedModelRenderer? ModelRenderer { get; set; }

	[Property] protected eAttachPoint AttachPoint { get; set; } = eAttachPoint.RightHand;

	[Property] protected eHoldType HoldType { get; set; } = eHoldType.Rifle;

	private Player? _owner;

	public Player? Owner
	{
		get => _owner ??= Scene.Directory.FindComponentByGuid( OwnerId ) as Player;
	}

	[HostSync] public Guid OwnerId { get; set; }

	[Sync] public bool IsDeployed { get; private set; }

	private ViewModel? viewModel { get; set; }

	private ViewModel? ViewModel
	{
		get => viewModel;
		set
		{
			viewModel = value;

			if ( viewModel.IsValid() )
			{
				viewModel.SetWeapon( this );
			}
		}
	}

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

		Log.Info( "Weapon started" );

		if ( !this.IsDeployed )
		{
			this.OnDeployed();
		}
		else
		{

		}

		// OnDeployed();
		// base.OnStart();
	}

	protected void ClearViewModel()
	{
		if ( ViewModel.IsValid() )
		{
			this.ViewModel.GameObject.Destroy();

			this.ViewModel = null;
		}
	}

	protected void CreateViewModel()
	{
		if ( !this.Owner.IsValid() )
		{
			return;
		}

		this.ClearViewModel();
		this.UpdateRenderMode();

		if ( this.Info.ViewModelPrefab.IsValid() )
		{
			Log.Info( "Weapon::CreateViewModel -> View model creating" );

			GameObject viewModelGameObject = this.Info.ViewModelPrefab.Clone( new CloneConfig()
			{
				Transform 	 = new(),
				Parent 		 = this.Owner.ViewModelCamera.GameObject,
				StartEnabled = true,
			});

			this.ViewModel = viewModelGameObject.Components.Get<ViewModel>();

			viewModelGameObject.BreakFromPrefab();
		}
	}

	protected void UpdateRenderMode()
	{
		bool enabled = this.Owner.IsValid() && !this.Owner.IsPossessed && this.IsDeployed;
	}

	protected virtual void OnDeployed()
	{
		if ( this.Owner.IsValid() && Owner.IsPossessed )
		{
			Log.Info( "Weapon::OnDeployed is possessed" );

			this.CreateViewModel();
		}

		this.UpdateRenderMode();
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
