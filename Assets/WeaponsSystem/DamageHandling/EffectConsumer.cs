using GameplayAbilities.Runtime.GameplayEffects;
using UnityEngine;

namespace WeaponsSystem.DamageHandling {
    public class EffectConsumer : MonoBehaviour, ISusceptible {
        [field: SerializeField] private GameplayEffectCoordinator GameplayEffectCoordinator { get; set; }
        
        public void HandleEffect(GameplayEffect effect) {
            this.GameplayEffectCoordinator.Add(effect, effect.Data.BaseChance);
        }
    }
}
