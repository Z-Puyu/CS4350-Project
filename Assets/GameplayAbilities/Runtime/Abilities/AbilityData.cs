using GameplayAbilities.Runtime.Attributes;
using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities {
    public record AbilityData(
        AbilityInfo Info,
        IAttributeReader Source,
        Vector3 Position,
        Transform TargetTransform,
        Transform SourceTransform
    );
}
