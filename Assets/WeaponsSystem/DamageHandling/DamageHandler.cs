using System.Collections.Generic;
using Common;
using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using UnityEngine;

namespace WeaponsSystem.DamageHandling {
    [DisallowMultipleComponent]
    public sealed class DamageHandler : MonoBehaviour {
        [field: SerializeField] private AbilitySystem AbilitySystem { get; set; }
        [field: SerializeField] private GameplayEffectCoordinator GameplayEffectCoordinator { get; set; }
        
        public void HandleDamage(Damage damage) {
            GameObject source = damage.Instigator;
            AbilitySystem instigator = source.GetComponentInChildren<AbilitySystem>();
            if (!instigator) {
                Debug.LogError($"{source.name} must have an Ability System to attack {this.gameObject.name}!", source);
                return;
            }

            if (!this.AbilitySystem) {
                return;
            }
#if DEBUG
            OnScreenDebugger.Log($"{source.name} damaged {this.gameObject.name}!");
#endif
            GameplayEffectExecutionArgs args = instigator.CreateEffectExecutionArgs()
                                                         .At(this.transform)
                                                         .WithUserData(damage.Data)
                                                         .Build();
            foreach (IAbility effect in damage.EffectsOnTarget) {
                instigator.Use(effect, this.AbilitySystem, args);
            }
            
            args = instigator.CreateEffectExecutionArgs().WithUserData(damage.Data).Build();
            foreach (IAbility effect in damage.EffectsOnSelf) {
                instigator.Use(effect, instigator, args);
            }
        }
    }
}
