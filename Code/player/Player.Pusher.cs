namespace Gamejam;
public partial class Player : Component
{
	[Property] public float Radius { get; set; } = 50;

	protected override void DrawGizmos()
	{
		base.DrawGizmos();

		Gizmo.Draw.LineSphere( Vector3.Zero.WithZ( 30f ), Radius );
	}

	public static Vector3 GetPushVector( in Vector3 position, Scene scene, GameObject ignore )
	{
		Vector3 vec = default;

		foreach ( var pusher in scene.GetAllComponents<Player>() )
		{
			if ( pusher.GameObject.IsAncestor( ignore ) )
				continue;

			pusher.Collect( position, ref vec );
		}

		return vec;
	}

	private void Collect( Vector3 position, ref Vector3 output )
	{
		var delta = (position - Transform.Position);
		if ( delta.Length > Radius ) return;

		delta.z = 0; // ignore z

		var distanceDelta = (delta.Length / Radius);

		output += delta.Normal * (1.0f - distanceDelta);
	}

}

