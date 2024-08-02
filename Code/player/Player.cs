
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

	[Property] public CitizenAnimationHelper AnimationHelper { get; set; }
	protected BoxCollider Collider;



	protected override void OnStart()
	{
		Camera = Components.Get<CameraComponent>( FindMode.EverythingInSelfAndDescendants );
		PlayerBody = Components.Get<SkinnedModelRenderer>( FindMode.EverythingInSelfAndDescendants );
		Collider = Components.Get<BoxCollider>( FindMode.EverythingInSelfAndDescendants );
	}


	protected override void OnUpdate()
	{
		
	}

	protected override void OnFixedUpdate()
	{
	}
	protected override void OnPreRender()
	{
		UpdateBodyVisibility();
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
