
using Sandbox;
using Sandbox.Citizen;
using Sandbox.Diagnostics;
using System;
using System.Linq;
using System.Numerics;

public partial class Player : Component
{
	public CameraComponent Camera;

	[Property]
	public GameObject EyePos;
	public SkinnedModelRenderer PlayerBody;
	public static Vector3 DebugCameraPosition { get; set; }
	public static Angles DebugCameraAngles { get; set; }

	public static bool HideHead { get; set; } = true;
	public static bool DebugCamera { get; set; } = false;
	public static Vector3 WishVelocity { get; private set; }


	protected override void OnStart()
	{
		Camera = Components.Get<CameraComponent>( FindMode.EverythingInSelfAndDescendants );
		PlayerBody = Components.Get<SkinnedModelRenderer>( FindMode.EverythingInSelfAndDescendants );
	}


	protected override void OnUpdate()
	{
		UpdateAnimation();
		UpdateCamera();
		ControllerOnUpdate();
	}

	protected override void OnFixedUpdate()
	{
		UpdateController();
	}
}
