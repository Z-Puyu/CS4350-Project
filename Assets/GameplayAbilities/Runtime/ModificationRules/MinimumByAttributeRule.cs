using System;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.ModificationRules {
    public class MinimumByAttributeRule : IAttributeModificationRule {
        [field: SerializeField, Required] private AttributeTypeDefinition Min { get; set; }
        
        public float Apply(float value, AttributeSet root) {
            if (this.Min) {
                return Math.Max(value, root.GetCurrent(this.Min.Id));
            }

            Debug.LogError("Max Attribute is not set");
            return value;
        }
    }
}
