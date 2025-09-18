using System;
using GameplayAbilities.Runtime.Attributes;
using UnityEngine;

namespace GameplayAbilities.Runtime.ModificationRules {
    [Serializable]
    public class MaximumByConstantRule : IAttributeClampRule {
        [field: SerializeField] private int Max { get; set; }

        public float MaxValueIn(AttributeSet root) {
            return this.Max;
        }
        
        public float MinValueIn(AttributeSet root) {
            return float.NegativeInfinity;
        }
    }
}
