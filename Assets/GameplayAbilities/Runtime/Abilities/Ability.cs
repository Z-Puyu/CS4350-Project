using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.GameplayEffects;
using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities {
    [CreateAssetMenu(fileName = "New Ability", menuName = "Gameplay Abilities/Ability", order = 0)]
    public class Ability : ScriptableObject, IAbility {
        [field: SerializeField] public List<GameplayEffectData> Effects { get; set; } = new List<GameplayEffectData>();

        public IEnumerable<GameplayEffect> GenerateEffects(GameplayEffectExecutionArgs args) {
            return this.Effects.Select(effect => new GameplayEffect(effect, args));
        }

        public bool IsUsable(AttributeSet instigator, AttributeSet target) {
            return true;
        }
    }
}
