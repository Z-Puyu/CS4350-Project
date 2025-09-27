using System.Collections.Generic;
using Common;
using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.GameplayEffects;
using GameplayAbilities.Runtime.HealthSystem;
using SaintsField;
using UnityEngine;
using WeaponsSystem.DamageHandling;

namespace Game.CharacterControls {
    [DisallowMultipleComponent]
    public sealed class DamageHandler : MonoBehaviour {
        [field: SerializeField] private AbilitySystem AbilitySystem { get; set; }
        [field: SerializeField, Required] private Ability DirectAttackAbility { get; set; }
        [field: SerializeField] private List<Ability> SpecialAttackAbilities { get; set; }
        [field: SerializeField] private GameplayEffectCoordinator GameplayEffectCoordinator { get; set; }
        [field: SerializeField] private Health Health { get; set; }
        
        public void HandleDamage(Damage damage) {
            GameObject source = damage.Instigator.root;
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
            int health = this.Health.Value;
            GameplayEffectExecutionArgs args = instigator.CreateEffectExecutionArgs()
                                                         .From(this.transform)
                                                         .WithUserData(damage.Data)
                                                         .Build();
            instigator.Use(this.DirectAttackAbility, this.AbilitySystem, args);
            int damageMagnitude = health - this.Health.Value;
            foreach (Ability effect in this.SpecialAttackAbilities) {
                instigator.Use(effect, this.AbilitySystem, args);
            }
        }
    }
}
