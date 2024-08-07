namespace Gamejam;

public class OpenDoor : Component, IContextActionProvider
{
	public Color GlowColor => Color.Green;
	public bool AlwaysGlow => true;
	public float InteractionRange => 100f;
	[Property] public SoundEvent ToggleSound { get; set; }
	private ContextAction GoToWorld { get; set; }
	public virtual string Title { get; set; } = "Loot Spawner";
	public Vector3 Position { get; set; }

	public string GetContextName()
	{
		return Title;
	}
	protected override void OnStart()
	{
		GoToWorld = new ContextAction( "home", "Make Home", "textures/crosshair.png" );
		Position = Transform.Position;
		Tags.Add("interaction");
	}

	public IEnumerable<ContextAction> GetSecondaryActions( Player player )
	{
		yield break;
	}

	public ContextAction GetPrimaryAction( Player player )
	{
		return GoToWorld;
	}

	public virtual void OnContextAction( Player player, ContextAction action )
	{
		if ( action == GoToWorld )
		{
			// if ( Game.IsServer )
			// {
			// 	var timedAction = new TimedActionInfo( Open );

			// 	timedAction.SoundName = OpeningSound;
			// 	timedAction.Title = "Opening";
			// 	timedAction.Origin = Position;
			// 	timedAction.Duration = 1f;
			// 	timedAction.Icon = "textures/ui/actions/open.png";

			// 	player.StartTimedAction( timedAction );
			// }
		}
	}

	protected override void OnUpdate()
	{
		
	}
}
