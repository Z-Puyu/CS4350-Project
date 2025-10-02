using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Targeting;
using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities {
    public interface IAbility {
        public bool IsFeasible(IAttributeReader instigator, AttributeSet target);
        
        public void Execute(IAttributeReader instigator, AttributeSet target);
        
        public void Activate(AbilityCaster caster, AbilityTargeter targeter);

        public void Delegate(GameObject carrier, AbilityCaster caster, AbilityTargeter targeter);
    }
}
