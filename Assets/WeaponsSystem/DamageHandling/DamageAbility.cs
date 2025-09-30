using System;
using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.Attributes;
using UnityEngine;

namespace WeaponsSystem.DamageHandling {
    [Serializable]
    public class DamageAbility : IAbility {
        [field: SerializeField] private DamageEffect DamageEffect { get; set; }
        
        public bool IsFeasible(IAttributeReader instigator, AttributeSet target) {
            return true;
        }
        
        public void Execute(IAttributeReader instigator, AttributeSet target) {
            this.DamageEffect.Apply(instigator, target).Start();
        }
    }
}
