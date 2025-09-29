using System.Collections.Generic;
using GameplayAbilities.Runtime.Abilities;
using SaintsField;
using UnityEngine;
using WeaponsSystem.DamageHandling;

namespace Game.CharacterControls {
    [DisallowMultipleComponent]
    public sealed class HitReaction : MonoBehaviour {
        /*[field: SerializeField] private List<Ability> ReflexiveAbilities { get; set; }
        [field: SerializeField, Required] private AbilitySystem AbilitySystem { get; set; }

        public void React(Damage data, int damage) {
            GameplayEffectExecutionArgs args = this.AbilitySystem
                                                   .CreateEffectExecutionArgs()
                                                   .WithUserData("Damage", damage)
                                                   .WithUserData(data.Data)
                                                   .Build();
            foreach (Ability ability in this.ReflexiveAbilities) {
                this.AbilitySystem.Use(ability, this.AbilitySystem, args);
            }
        }*/
    }
}
