using System;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Modifiers;
using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities {
    [Serializable]
    public struct PerkModifier {
        [field: SerializeField] private AttributeType TargetAttribute { get; set; }
        [field: SerializeField] private int Magnitude { get; set; }
        [field: SerializeField] private bool IsMultiplier { get; set; }
        
        public Modifier ToModifier() {
            return new Modifier(
                this.Magnitude, this.IsMultiplier ? Modifier.Operation.Multiply : Modifier.Operation.Shift,
                this.TargetAttribute.Id
            );
        }
    }
}
