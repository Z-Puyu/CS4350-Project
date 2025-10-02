using System;
using DataStructuresForUnity.Runtime.Utilities;
using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Targeting;
using GameplayEffects.Runtime;
using UnityEngine;

namespace WeaponsSystem.Runtime.DamageHandling {
    [Serializable]
    public sealed class DamageAbility : IAbility {
        [field: SerializeReference] private IEffect<IDataReader<string, int>, AttributeSet> Effect { get; set; }
        
        public bool IsFeasible(IAttributeReader instigator, AttributeSet target) {
            return true;       
        }
        
        public void Execute(IAttributeReader instigator, AttributeSet target) {
            this.Effect.Apply(instigator, target).Start();
        }

        public void Activate(AbilityCaster caster, AbilityTargeter targeter) { }
        
        public void Delegate(GameObject carrier, AbilityCaster caster, AbilityTargeter targeter) { }
    }
}
