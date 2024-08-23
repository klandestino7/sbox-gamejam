

namespace Gamejam;
public record DamageDataInfo(
    int amount,
    DamageType type,
    int boneIndex,
    GameObject attacker = null,
    Vector3 forceDirection = default,
    float force = 0
) {

    public Component Victim { get; set; }

    public float DamageTime = 0;
}