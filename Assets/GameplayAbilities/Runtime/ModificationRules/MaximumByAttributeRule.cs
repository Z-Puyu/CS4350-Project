using System;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.ModificationRules {
    [Serializable]
    public class MaximumByAttributeRule : IAttributeClampRule {
        [field: SerializeField, Required] private AttributeType Max { get; set; }

        public int MaxValueIn(AttributeSet root) {
            return root.GetCurrent(this.Max.Id);
        }
        
        public int MinValueIn(AttributeSet root) {
            return int.MinValue;
        }
    }
}
