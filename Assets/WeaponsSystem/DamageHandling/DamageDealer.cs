using System.Collections.Generic;
using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.GameplayEffects;
using UnityEngine;

namespace WeaponsSystem.DamageHandling {
    [DisallowMultipleComponent]
    public sealed class DamageDealer : MonoBehaviour {
        [field: SerializeField] private Ability DamageDealingAbility { get; set; }

        private GameplayEffectExecutionArgs GenerateDamageArgs(
            AbilitySystem instigator, IDictionary<string, int> damages
        ) {
            return instigator.CreateEffectExecutionArgs()
                             .WithUserData(damages)
                             .Build();
        }

        public void Damage(AbilitySystem target, IDictionary<string, int> damages) {
            AbilitySystem instigator = this.GetComponentInParent<AbilitySystem>();
            GameplayEffectExecutionArgs args = this.GenerateDamageArgs(instigator, damages);
            instigator.Use(this.DamageDealingAbility, target, args);
        }
    }
}
