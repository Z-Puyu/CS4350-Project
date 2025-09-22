using System;
using GameplayAbilities.Runtime.Attributes;
using UnityEngine;

namespace GameplayAbilities.Runtime.ModificationRules {
    [Serializable]
    public class MaximumByConstantRule : IAttributeClampRule {
        [field: SerializeField] private int Max { get; set; }

        public int MaxValueIn(AttributeSet root) {
            return this.Max;
        }
        
        public int MinValueIn(AttributeSet root) {
            return int.MinValue;
        }
    }
}
