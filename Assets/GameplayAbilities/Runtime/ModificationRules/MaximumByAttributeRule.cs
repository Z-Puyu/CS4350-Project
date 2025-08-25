using System;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.ModificationRules {
    [Serializable]
    public class MaximumByAttributeRule : IAttributeModificationRule {
        [field: SerializeField, Required] private AttributeTypeDefinition Max { get; set; }
        
        public float Apply(float value, AttributeSet root) {
            if (this.Max) {
                return Math.Min(value, root.GetCurrent(this.Max.Id));
            }

            Debug.LogError("Max Attribute is not set");
            return value;
        }
    }
}
