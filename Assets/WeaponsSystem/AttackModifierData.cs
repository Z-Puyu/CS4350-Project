using System;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Modifiers;
using UnityEngine;

namespace WeaponsSystem {
    [Serializable]
    public struct AttackModifierData {
        [field: SerializeField] public AttributeTypeDefinition Target { get; private set; }
        [field: SerializeField] public Modifier.Operation Type { get; private set; }
        [field: SerializeField] public int Magnitude { get; private set; }
    }
}
