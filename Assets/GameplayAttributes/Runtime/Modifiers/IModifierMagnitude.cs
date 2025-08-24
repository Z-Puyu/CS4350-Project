namespace GameplayAttributes.Runtime.Modifiers {
    internal interface IModifierMagnitude {
        internal float Evaluate(AttributeSet target, IAttributeReader instigator);
    }
}
