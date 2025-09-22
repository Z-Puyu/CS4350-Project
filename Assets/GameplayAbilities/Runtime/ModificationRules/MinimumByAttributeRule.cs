using System;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.ModificationRules {
    public class MinimumByAttributeRule : IAttributeClampRule {
        [field: SerializeField, Required] private AttributeTypeDefinition Min { get; set; }

        public int MaxValueIn(AttributeSet root) {
            return int.MaxValue;
        }
        
        public int MinValueIn(AttributeSet root) {
            return root.GetCurrent(this.Min.Id);
        }
    }
}
