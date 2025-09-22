using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.Attributes;
using UnityEngine;

namespace GameplayAbilities.Runtime.ModificationRules {
    [Serializable]
    public class MinimumByAttributeSumRule : IAttributeClampRule {
        [field: SerializeField]
        private List<MinimumByAttributeRule> Rules { get; set; } = new List<MinimumByAttributeRule>();
        
        public int MaxValueIn(AttributeSet root) {
            return int.MaxValue;
        }
        
        public int MinValueIn(AttributeSet root) {
            return this.Rules.Sum(rule => rule.MinValueIn(root));
        }
    }
}
