using Sandbox;
using Sandbox.Citizen;
using Sandbox.Diagnostics;

namespace Gamejam;

public partial class NPC : Component
{
	public static bool Debug { get; set; } = false;

	[Property] public virtual HealthSystem HealthSystem { get; set; }
	[Property] public CitizenAnimationHelper AnimationHelper { get; set; }
	[Property] public SkinnedModelRenderer Body;
	[Property] public CharacterController CharacterController { get; set; }
	[Property] public bool IsStatic { get; set; } = false;

	[Property]
	[Range( 0f, 1024f, 16f, false )]
	public float MaxDetectDistance { get; set; } = 256f;

	[Property]
	// [Category( "Stats" )]
	public bool ScaleStats { get; set; } = true;
	public float ObjectScale => ScaleStats ? MathF.Max( MathF.Max( GameObject.Transform.Scale.x, GameObject.Transform.Scale.y ), GameObject.Transform.Scale.z ) : 1f;

	[Property]
	public TagSet EnemyTags { get; set; }

	[Property]
	// [Category( "Stats" )]
	[Range( 0f, 2024f, 16f, false )]
	public float MaxDistanceToLostTarget { get; set; } = 512f;

	public delegate void NpcTrigger( GameObject enemy );

	/// <summary>
	/// When the NPC is first spawned in
	/// </summary>
	[Property]
	[Category( "Triggers" )]
	public Action OnSpawn { get; set; }

	/// <summary>
	/// When an enemy enters the detect area or attacks the NPC, or a nearby NPC gets alerted. This won't get called if the NPC has been alerted already.
	/// </summary>
	[Property]
	[Category( "Triggers" )]
	public NpcTrigger OnDetect { get; set; }

	/// <summary>
	/// When the enemy is within attack range and our cooldown is up
	/// </summary>
	[Property]
	[Category( "Triggers" )]
	public NpcTrigger OnAttack { get; set; }

	/// <summary>
	/// When the enemy gets out of the Vision Range or dies
	/// </summary>
	[Property]
	[Category( "Triggers" )]
	public NpcTrigger OnEnemyEscaped { get; set; }

	/// <summary>
	/// When the NPC dies, enemy will be NULL if it died of natural causes (???)
	/// </summary>
	[Property]
	[Category( "Triggers" )]
	public NpcTrigger OnKilled { get; set; }

	/// <summary>
	/// When the NPC has no target it will occasionally fire this off
	/// </summary>
	[Property]
	[Category( "Triggers" )]
	public Action OnIdle { get; set; }
    
	[Property] public GameObject TargetObject { get; private set; } = null;


	protected bool IsPatrolling { get; private set; }

	[HostSync] public int NpcId { get; set; }
	[Property, HostSync] public String Name { get; set; } = "test";
	[HostSync] public Vector3 SpawnPosition { get; set; }
	[HostSync] public Vector3 TargetPosition { get; set; }
	public Collider Collider { get; private set; }

	/// <summary>
	/// How far the NPC can reach to attack with the target
	/// </summary>
	[Property]
	[Category( "Stats" )]
	[Range( 30f, 200f, 10f, false )]
	public float AttackRange { get; private set; } = 80f;
	/// <summary>
	/// How many seconds must pass before the NPC can attack with the target again
	/// </summary>
	[Property]
	[Category( "Stats" )]
	[Range( 0.5f, 10f, 0.1f, false )]
	public float AttackCooldown { get; private set; } = 5f;
	[HostSync] public TimeUntil NextAttack { get; set; }

	protected override void OnStart()
	{
		Tags.Add( "npc" );

	}

	protected override void OnEnabled()
	{
		Tags.Set( "npc", true );
        
		if ( Body != null )
			Body.OnFootstepEvent += OnEventFootstep;

        
		NpcId = Scene.GetAllComponents<NPC>().OrderByDescending( x => x.NpcId ).First().NpcId + 1;

		Collider = Components.Get<Collider>();
	}

	protected override void OnDisabled()
	{
		if ( Body is null )
			return;

		Body.OnFootstepEvent -= OnEventFootstep;
	}


    protected override void OnAwake()
	{
		var spawnTrace = Scene.Trace.Ray( Transform.Position + Vector3.Up * 30f, Transform.Position - Vector3.Up * 200f )
			.Size( 5f )
			.IgnoreGameObjectHierarchy( GameObject )
			.WithoutTags( "player", "npc", "trigger" )
			.Run();

		SpawnPosition = spawnTrace.Hit ? spawnTrace.HitPosition : Transform.Position;
		BroadcastOnSpawn();
	}

	public virtual string GetDisplayName()
	{
		return "NPC";
	}

	public virtual float GetMoveSpeed()
	{
		return 80f;
	}

	protected override void OnUpdate()
	{
		UpdateAnimation();
	}

	public void SearchPlayerTarget()
	{
		if ( TargetObject != null )
		{
			if ( IsWithinRange( TargetObject ) )
			{
				if ( NextAttack )
				{
					BroadcastOnAttack();
					NextAttack = AttackCooldown;
				}
			}
		}

		var playersClosest = Scene.GetAllComponents<Player>()
			.Where( x => x.IsValid() && x.GameObject.Enabled )
			.Where( x => !x.HealthSystem.IsValid() || !x.HealthSystem.IsDead )
			.Where( x => x.Transform.Position.Distance( Transform.Position ) <= MaxDetectDistance * ObjectScale )
			.Select( x => x.GameObject );

		if ( TargetObject == null )
		{
			var nearest = playersClosest.FirstOrDefault();
			if ( nearest != null )
			{
				LockTarget( nearest, true );
				return;
			}
		}

		if ( TargetObject != null && TargetObject.IsValid() )
		{
			var targetDead = TargetObject.Components.Get<HealthSystem>()?.IsDead ?? false;
			var targetEscaped = TargetObject.Transform.Position.Distance( Transform.Position ) > MaxDistanceToLostTarget * ObjectScale;

			if ( targetEscaped || targetDead )
				UnlockTarget();
		}
	}

	public void LockTarget( GameObject target, bool alertOthers = false )
	{
		TargetObject = target;
	}

	public void UnlockTarget( )
	{
		TargetObject = null;
	}

	protected override void OnFixedUpdate()
	{
		if ( !IsStatic )
		{
			FollowTarget();
			UpdateMovement();
		}

		SearchPlayerTarget();
	}

	public bool IsWithinRange( GameObject target )
	{
		if ( !GameObject.IsValid() ) return false;

		return IsWithinRange( target, AttackRange * ObjectScale );
	}

	public bool IsWithinRange( GameObject target, float range = 60f )
	{
		if ( !GameObject.IsValid() ) return false;

		return target.Transform.Position.Distance( Transform.Position ) <= range;
	}

	TimeSince timeSinceStep;

	private void OnEventFootstep( SceneModel.FootstepEvent e )
	{
		if ( timeSinceStep < 0.2f )
			return;

		var tr = Scene.Trace
			.Ray( e.Transform.Position + Vector3.Up * 20, e.Transform.Position + Vector3.Up * -20 )
			.Run();

		if ( !tr.Hit )
			return;

		if ( tr.Surface is null )
			return;

		timeSinceStep = 0;

		var sound = e.FootId == 0 ? tr.Surface.Sounds.FootLeft : tr.Surface.Sounds.FootRight;
		if ( sound is null ) return;

		var handle = Sound.Play( sound, tr.HitPosition + tr.Normal * 5 );
		handle.Volume *= e.Volume;
	}

	[Broadcast]
	private void BroadcastOnSpawn()
	{
		OnSpawn?.Invoke();
	}

    
	[Broadcast]
	private void BroadcastOnIdle()
	{
		OnIdle?.Invoke();
	}

	[Property, Category( "Stats" )]
	[Range( 1f, 100f, 1f )]
	public float AttackDamage { get; set; } = 10f;

	[Broadcast]
	private void BroadcastOnAttack()
	{
		if ( TargetObject is null ) return;

		OnAttack?.Invoke( TargetObject );

		var targetHealth = TargetObject.Components.Get<HealthSystem>( FindMode.EverythingInSelfAndDescendants );
		if ( targetHealth.IsValid() )
		{
			targetHealth.ApplyDamage( new DamageDataInfo(
				amount: (int)AttackDamage,
				type: DamageType.Punch,
				boneIndex: 0,
				attacker: GameObject,
				forceDirection: Transform.Position
			) );
		}
	}

	[Broadcast]
	private void BroadcastOnDetect()
	{
		if ( TargetObject is not null )
			OnDetect?.Invoke( TargetObject );
	}

	[Broadcast]
	private void BroadcastOnEscape()
	{
		if ( TargetObject is not null )
			OnEnemyEscaped?.Invoke( TargetObject );
	}

	public void SearchTarget()
	{

	}

	[ConCmd( "spawn_npc" )]
	public static void DebugSpawnNPC( string name )
	{
		var allNpcs = PrefabLibrary.FindByComponent<NPC>();

		var foundNpc = allNpcs
			.Where( p =>
			{
				Log.Info( $"npcname {  p.GetComponent<NPC>().Get<string>("Name").ToLower() } other= { name.ToLower() }" );

				return p.GetComponent<NPC>().Get<string>("Name").ToLower() == name.ToLower();
			}
			).FirstOrDefault();
	

		if ( foundNpc != null )
		{
			var obj = SceneUtility.GetPrefabScene( foundNpc.Prefab ).Clone();
			obj.NetworkMode = NetworkMode.Object;
			obj.NetworkSpawn();
			var aimRay = Player.Local.CameraController.AimRay;
			var trace = Sandbox.Game.ActiveScene.Trace.Ray( aimRay.Position, aimRay.Position + aimRay.Forward * 1000f )
				.WithoutTags( "player", "npc", "trigger" )
				.IgnoreGameObjectHierarchy( Player.Local.GameObject )
				.Run();

			obj.Transform.Position = trace.Hit ? trace.HitPosition : trace.EndPosition;
		}
		else
		{
			Log.Info( $"The npc was not found, here is a list of available npcs:" );

			var availableNpcs = "";

			foreach ( var availabelNpc in allNpcs )
				availableNpcs += $"[{availabelNpc.GetComponent<NPC>().Get<string>( "Name" )}], ";

			Log.Info( availableNpcs );
			Log.Info( "You may also use partial npc names or any combination of words and letters, I'll try my best to find the npc." );
		}
	}
}
