using System;
using UnityEngine;

namespace GameplayAttributes.Runtime.Modifiers {
    [Serializable]
    public struct Constant : IModifierMagnitude {
        [field: SerializeField] private int Value { get; set; }
        
        float IModifierMagnitude.Evaluate(AttributeSet target, IAttributeReader instigator) {
            return this.Value;
        }
    }
}
