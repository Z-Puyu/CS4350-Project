using GameplayAbilities.Runtime.Attributes;

namespace GameplayAbilities.Runtime.Modifiers {
    internal interface IModifierMagnitude {
        internal float Evaluate(AttributeSet target, IAttributeReader instigator);
    }
}
