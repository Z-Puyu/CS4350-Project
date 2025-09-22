using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.GameplayEffects;
using UnityEngine;

namespace WeaponsSystem.DamageHandling {
    [DisallowMultipleComponent]
    public sealed class DamageHandler : MonoBehaviour {
        [field: SerializeField] private bool IsInvincible { get; set; }
        [field: SerializeField] private AbilitySystem AbilitySystem { get; set; }
        
        private void HandleDamage(Damage damage) {
            GameObject source = damage.Instigator;
            AbilitySystem instigator = source.GetComponentInChildren<AbilitySystem>();
            if (!instigator) {
                Debug.LogError($"{source.name} must have an Ability System to attack the player!", source);
            } else {
                GameplayEffectExecutionArgs args = instigator.CreateEffectExecutionArgs()
                                                             .WithUserData(damage.Data)
                                                             .Build();
                instigator.Use("basic:attack", this.AbilitySystem, args);
            }
        }
    }
}
