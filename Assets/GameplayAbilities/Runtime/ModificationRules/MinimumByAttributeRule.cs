using System;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.ModificationRules {
    public class MinimumByAttributeRule : IAttributeClampRule {
        [field: SerializeField, Required] private AttributeTypeDefinition Min { get; set; }

        public float MaxValueIn(AttributeSet root) {
            return float.PositiveInfinity;
        }
        
        public float MinValueIn(AttributeSet root) {
            return root.GetCurrent(this.Min.Id);
        }
    }
}
