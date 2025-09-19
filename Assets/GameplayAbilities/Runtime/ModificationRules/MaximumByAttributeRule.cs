using System;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.ModificationRules {
    [Serializable]
    public class MaximumByAttributeRule : IAttributeClampRule {
        [field: SerializeField, Required] private AttributeTypeDefinition Max { get; set; }

        public float MaxValueIn(AttributeSet root) {
            return root.GetCurrent(this.Max.Id);
        }
        
        public float MinValueIn(AttributeSet root) {
            return float.NegativeInfinity;
        }
    }
}
