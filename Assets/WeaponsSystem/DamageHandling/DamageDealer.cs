using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.GameplayEffects;
using UnityEngine;

namespace WeaponsSystem.DamageHandling {
    [DisallowMultipleComponent]
    public sealed class DamageDealer : MonoBehaviour {
        [field: SerializeField] private Ability DamageDealingAbility { get; set; }
        [field: SerializeField] 

        private GameplayEffectExecutionArgs GenerateDamageArgs(AbilitySystem instigator) {
            return instigator.CreateEffectExecutionArgs().Build();
        }

        public void Damage(AbilitySystem instigator, AbilitySystem target) {
            GameplayEffectExecutionArgs args = this.GenerateDamageArgs(instigator);
            instigator.Use(this.DamageDealingAbility, target, args);
        }
    }
}
