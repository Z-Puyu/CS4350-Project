using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.Attributes;
using UnityEngine;

namespace GameplayAbilities.Runtime.ModificationRules {
    [Serializable]
    public class CompositeRule : IAttributeClampRule {
        [field: SerializeField, Tooltip("The clamped attribute will need to satisfy all of these rules")] 
        private List<IAttributeClampRule> Rules { get; set; }

        public float MaxValueIn(AttributeSet root) {
            return this.Rules.Min(rule => rule.MaxValueIn(root));
        }
        
        public float MinValueIn(AttributeSet root) {
            return this.Rules.Max(rule => rule.MinValueIn(root));
        }
    }
}
