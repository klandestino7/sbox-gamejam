using Sandbox;
using System;
using System.Collections.Generic;

namespace Gamejam;

public interface IContextActionProvider : IValid
{
	public static IEnumerable<ContextAction> GetAllActions( Player player, IContextActionProvider provider )
	{
		var primary = provider.GetPrimaryAction( player );

		if ( primary.IsValid() )
		{
			yield return primary;
		}

		var secondary = provider.GetSecondaryActions( player );

		foreach ( var action in secondary )
		{
			yield return action;
		}
	}

	// public IComponentSystem Components { get; }
	public float InteractionRange { get; }
	public bool AlwaysGlow { get; }
	public Color GlowColor { get; }
	public IEnumerable<ContextAction> GetSecondaryActions( Player player );
	// public int NetworkIdent { get; }
	public ContextAction GetPrimaryAction( Player player );
	public Vector3 Position { get; }
	public string GetContextName();
	public void OnContextAction( Player player, ContextAction action );
}
