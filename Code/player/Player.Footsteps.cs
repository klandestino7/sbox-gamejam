namespace Gamejam;
public partial class Player : Component
{

	protected override void OnEnabled()
	{
		if ( PlayerBody is null )
			return;

		PlayerBody.OnFootstepEvent += OnEvent;
	}

	protected override void OnDisabled()
	{
		if ( PlayerBody is null )
			return;

		PlayerBody.OnFootstepEvent -= OnEvent;
	}

	TimeSince timeSinceStep;

	private void OnEvent( SceneModel.FootstepEvent e )
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

		var positionFootHit = tr.HitPosition + tr.Normal * 5;
		var addictional = ViewModelCamera.Transform.Position - positionFootHit;

		var sound = e.FootId == 0 ? tr.Surface.Sounds.FootLeft : tr.Surface.Sounds.FootRight;
		if ( sound is null ) return;

		var handle = Sound.Play( sound, tr.HitPosition + tr.Normal * 5 );
		handle.Volume *= e.Volume;
		handle.Update();
	}
}

