using GameplayAbilities.Runtime.Attributes;

namespace GameplayAbilities.Runtime.GameplayEffects {
    public interface IAbility {
        public bool IsFeasible(IAttributeReader instigator, AttributeSet target);
        
        public void Execute(IAttributeReader instigator, AttributeSet target);
    }
}
