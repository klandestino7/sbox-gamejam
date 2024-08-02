namespace Gamejam;
public partial class Player : Component
{
    public ModelPhysics Ragdoll => PlayerBody.Components.Get<ModelPhysics>();
	public bool IsRagdolled => Ragdoll.IsValid();
}

