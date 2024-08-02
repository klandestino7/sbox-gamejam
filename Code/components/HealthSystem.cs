using Sandbox;

namespace Gamejam;
public enum DamageType {
    Punch = 1,
    Weapon = 2,
    Explosion = 3,
    Fire = 4,
    Fall = 5
}

public sealed class HealthSystem : Component
{
	[Property] public bool Invincible { get; set; } = false;

	public int MaxHealth { get; set; } = 100;
	public int MaxStamina { get; set; } = 100;
	public int MaxOxigen { get; set; } = 100;

    public int Health { get; set; }
    public int Oxigen { get; set; }
    public bool IsDead { get; set; } = false;

    private float lastDamage { get; set; }


	protected override void OnStart()
    {
        Health = MaxHealth;
        Oxigen = MaxOxigen;
    }

    public void ApplyDamage( DamageInfo damageInfo )
    {
        if ( IsDead ) return;
        if ( Invincible ) return; 
        
        var playerPosition = Transform.World.Position;
        var forceDir = playerPosition - damageInfo.forceDirection.WithZ(0f).Normal;

        Health -= damageInfo.amount;
        lastDamage = Time.Delta;

        var playerBody = GameObject.Components.Get<Rigidbody>( FindMode.EverythingInSelf );
        playerBody.ApplyForce( forceDir );

        if ( Health >= 0 ) return;
        
        Health = 0;
        IsDead = true;

        Kill( damageInfo );
    }

    public void Kill( DamageInfo damageInfo ) {
        
    }

    public void DecreaseHealthByDamageType( DamageType type ) {

    }
    
	protected override void OnFixedUpdate()
    {
        
    }
}