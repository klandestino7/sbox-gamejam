
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
	[Sync] public bool ForceHidePenoid { get; set; } = false;

	public static bool HideHead { get; set; } = true;
	public static bool DebugCamera { get; set; } = false;
	public static Vector3 WishVelocity { get; private set; }

	protected BoxCollider Collider;


	bool _blockInputs = false;

	/// <summary>
	/// Block inputs (Like WASD, Pissing, Left/Right click)
	/// </summary>
	[Sync]
	public bool BlockInputs
	{
		get => BlockMovements || _blockInputs;
		set => _blockInputs = value;
	}


	protected override void OnStart()
	{
		Camera = Components.Get<CameraComponent>( FindMode.EverythingInSelfAndDescendants );
		PlayerBody = Components.Get<SkinnedModelRenderer>( FindMode.EverythingInSelfAndDescendants );
		Collider = Components.Get<BoxCollider>( FindMode.EverythingInSelfAndDescendants );
	}


	protected override void OnUpdate()
	{
		UpdateAnimation();
		UpdateCamera();
		// ControllerOnUpdate();

		if ( !IsProxy )
		{
			UpdateAngles();
		}
	}

	protected override void OnFixedUpdate()
	{
		// UpdateMovement();
		// UpdateController();
	}
}
