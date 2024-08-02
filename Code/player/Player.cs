using Sandbox;
using Sandbox.Citizen;

namespace Gamejam;

public partial class Player : Component
{
	public CameraComponent Camera;

	[Property] public GameObject EyePos;
	[Property] public SkinnedModelRenderer PlayerBody;
	[Property] public CharacterController CharacterController { get; set; }
	[Property] public CitizenAnimationHelper AnimationHelper { get; set; }
	
	[Property] public virtual HealthSystem HealthSystem { get; set; }
	protected BoxCollider Collider;

	protected override void OnStart()
	{
		Camera = Scene.GetAllComponents<CameraComponent>().Where( x => x.IsMainCamera ).FirstOrDefault();
		PlayerBody = Components.Get<SkinnedModelRenderer>( FindMode.EverythingInSelfAndDescendants );
		Collider = Components.Get<BoxCollider>( FindMode.EverythingInSelfAndDescendants );
	}

	protected override void OnUpdate()
	{
		if ( !IsProxy )
		{
			MouseInput();
			Transform.Rotation = new Angles( 0, EyeAngles.yaw, 0 );
		}

		UpdateAnimation();
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy )
			return;

		CrouchingInput();
		MovementInput();
	}
	protected override void OnPreRender()
	{
		UpdateBodyVisibility();
		
		if ( IsProxy )
			return;

		UpdateCamera();
	}

	private void UpdateBodyVisibility()
	{
		if ( AnimationHelper is null )
			return;

		var renderMode = ModelRenderer.ShadowRenderType.On;
		if ( !IsProxy ) renderMode = ModelRenderer.ShadowRenderType.ShadowsOnly;

		AnimationHelper.Target.RenderType = renderMode;

		var playerModels = GameObject.Components.GetAll<ModelRenderer>( FindMode.InChildren );

		foreach ( var clothing in playerModels )
		{
			if ( !clothing.Tags.Has( "clothing" ) )
				continue;

			clothing.RenderType = renderMode;
		}
	}
}
