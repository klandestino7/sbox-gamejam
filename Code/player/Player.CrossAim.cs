using Sandbox;
using Sandbox.Citizen;
using Sandbox.Diagnostics;
using System;
using System.Linq;
using System.Numerics;

public sealed class PlayerCrossAim : Component
{
	public Rotation lookToRotation;
	[Property] public GameObject Body { get; set; }
	public Vector3 cursorDirection { get; private set; }

	public Vector3 m_v3PawnCursorDir { get; private set ; }

    private bool playerOnMovement { get; set;}

	public bool aiming { get; private set;}
	protected override void OnUpdate()
	{
		var cursor = GameObject.Components.Get<Cursor>();
		
		
        var runButtonPressed = Input.Down( "run" );
		var attackButtonPressed = Input.Down( "attack1" );
		var aimButtonPressed = Input.Down( "attack2" );

		var eyeHeight = Body.Transform.Position.z + 50f;

		var cursorTraceStartPos = new Vector3( Body.Transform.Position.x, Body.Transform.Position.y, eyeHeight );
		var position = Scene.Camera.ScreenPixelToRay( Mouse.Position );
		var cursorPosition = new Vector3(position.Forward.x, position.Forward.y, position.Forward.z );

		var cursorTraceEndPos = cursorTraceStartPos + ( cursorPosition * 100.0f);

		
		// var tr = Scene.Trace.Ray( cursorTraceEndPos, cursorTraceStartPos )
		// 		.Run();

		// if (aimButtonPressed) {
		// 	Body.Transform.Rotation = Body.Transform.Rotation;
		// }

		// CursorPosition = new Vector3( Mouse.Position.x, Mouse.Position.y, 0f );
		// // var ray = Scene.Camera.ScreenPixelToRay( CursorPosition.ToScreen() );
		// DebugOverlay.Line( cursorTraceEndPos, EyePosition, Color.Red );
		// Gizmo.Draw.Line( Body.Transform.Position, Mouse.Position );
	
		// var playerPosition = new Vector3( Body.Transform.Position.x, Body.Transform.Position.y, eyeHeight );
		// var cursorPosition = new Vector3( cursor.CursorPosition.x, cursor.CursorPosition.y, eyeHeight );

        // Gizmo.Draw.Line( playerPosition, cursorPosition );
	}

	protected override void OnFixedUpdate()
	{
        if ( IsProxy )
			return;

		var cc = GameObject.Components.Get<CharacterController>();
		playerOnMovement = cc.Velocity.Length >= 1f;
	}
}
