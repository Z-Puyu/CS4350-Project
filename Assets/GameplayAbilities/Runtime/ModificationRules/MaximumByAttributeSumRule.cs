using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.Attributes;
using UnityEngine;

namespace GameplayAbilities.Runtime.ModificationRules {
    [Serializable]
    public class MaximumByAttributeSumRule : IAttributeClampRule {
        [field: SerializeField]
        private List<MaximumByAttributeRule> Rules { get; set; } = new List<MaximumByAttributeRule>();

        public int MaxValueIn(AttributeSet root) {
            return this.Rules.Sum(rule => rule.MaxValueIn(root));
        }
        
        public int MinValueIn(AttributeSet root) {
            return int.MinValue;
        }
    }
}
