using GameplayAbilities.Runtime.Attributes;

namespace GameplayAbilities.Runtime.Abilities {
    public interface IAbilityTarget {
        public void Receive(IAbility ability, IAttributeReader from);
    }
}
