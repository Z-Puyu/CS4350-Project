using GameplayAbilities.Runtime.Attributes;

namespace GameplayAbilities.Runtime.ModificationRules {
    public interface IAttributeClampRule {
        public float MaxValueIn(AttributeSet root);
        public float MinValueIn(AttributeSet root);
    }
}
