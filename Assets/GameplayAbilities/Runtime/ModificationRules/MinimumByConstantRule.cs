using System;
using GameplayAbilities.Runtime.Attributes;
using UnityEngine;

namespace GameplayAbilities.Runtime.ModificationRules {
    [Serializable]
    public class MinimumByConstantRule : IAttributeClampRule {
        [field: SerializeField] private int Min { get; set; }

        public float MaxValueIn(AttributeSet root) {
            return float.PositiveInfinity;
        }
        
        public float MinValueIn(AttributeSet root) {
            return this.Min;
        }
    }
}
