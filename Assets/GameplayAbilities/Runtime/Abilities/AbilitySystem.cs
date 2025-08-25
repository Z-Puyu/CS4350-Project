using System;
using System.Collections.Generic;
using GameplayAbilities.Runtime.GameplayEffects;
using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities {
    [DisallowMultipleComponent, RequireComponent(typeof(AttributeSet))]
    public class AbilitySystem : MonoBehaviour {
        private HashSet<IAbility> Abilities { get; } = new HashSet<IAbility>();
        private AttributeSet AttributeSet { get; set; }

        private void Awake() {
            this.AttributeSet = this.GetComponent<AttributeSet>();
        }

        public void Grant(IAbility ability) {
            this.Abilities.Add(ability);
        }

        public void Revoke(IAbility ability) {
            this.Abilities.Remove(ability);
        }
        
        public void Use(IAbility ability, AttributeSet target) {
            if (!this.Abilities.Contains(ability) || !ability.IsUsable(this.AttributeSet, target)) {
                return;
            }
            
            ability.Begin();
            foreach (GameplayEffect effect in ability.GenerateEffects(this.CreateEffectExecutionArgs())) {
                target.AddEffect(effect, effect.Data.BaseChance);
            }
            
            ability.End();
        }

        private GameplayEffectExecutionArgs CreateEffectExecutionArgs() {
            return GameplayEffectExecutionArgs.Builder.From(this.AttributeSet).Build();
        }
    }
}
