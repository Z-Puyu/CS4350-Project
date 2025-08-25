using System.Collections.Generic;
using GameplayAbilities.Runtime.GameplayEffects;

namespace GameplayAbilities.Runtime.Abilities {
    public interface IAbility {
        public List<GameplayEffectData> Effects { get; }
        public bool IsUsable(AttributeSet instigator);
    }
}
