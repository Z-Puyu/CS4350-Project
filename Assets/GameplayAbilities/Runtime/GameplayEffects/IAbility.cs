using GameplayAbilities.Runtime.Attributes;

namespace GameplayAbilities.Runtime.GameplayEffects {
    public interface IAbility {
        public void Execute(IAttributeReader instigator, AttributeSet target);
    }
}
