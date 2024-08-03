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
	public float MaxStamina { get; set; } = 100f;
	public int MaxOxigen { get; set; } = 100;

    public int Health { get; set; }
    public int Oxigen { get; set; }
    public float Stamina { get; set; }
    public bool IsDead { get; set; } = false;

    private float lastDamage { get; set; }
    private float lastStaminaDrainTimedelta { get; set; }
    [Property] private float staminaDrainTimeout = 1f;


	protected override void OnStart()
    {
        Health = MaxHealth;
        Oxigen = MaxOxigen;
        Stamina = MaxStamina;
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

    public void Kill( DamageInfo damageInfo )
    {
        
    }

    public void DrainStamina( float amount = 0.5f)
    {
        Stamina -= Math.Clamp( amount, 0, MaxStamina);
        // Stamina = Stamina.LerpTo( Stamina - amount, RealTime.Delta * 100f);
        // lastStaminaDrainTimedelta = Time.Delta + ( staminaDrainTimeout );
    }

    public void DecreaseHealthByDamageType( DamageType type )
    {

    }
    
	protected override void OnFixedUpdate()
    {

    }
}