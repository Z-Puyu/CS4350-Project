using GameplayAbilities.Runtime.Attributes;

namespace GameplayAbilities.Runtime.ModificationRules {
    public interface IAttributeClampRule {
        public int MaxValueIn(AttributeSet root);
        public int MinValueIn(AttributeSet root);
    }
}
