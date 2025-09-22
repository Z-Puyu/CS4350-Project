using System;
using GameplayAbilities.Runtime.Attributes;
using UnityEngine;

namespace GameplayAbilities.Runtime.ModificationRules {
    [Serializable]
    public class MinimumByConstantRule : IAttributeClampRule {
        [field: SerializeField] private int Min { get; set; }

        public int MaxValueIn(AttributeSet root) {
            return int.MaxValue;
        }
        
        public int MinValueIn(AttributeSet root) {
            return this.Min;
        }
    }
}
