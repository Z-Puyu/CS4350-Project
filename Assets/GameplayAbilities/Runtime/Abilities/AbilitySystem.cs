using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using GameplayAbilities.Runtime.Modifiers;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilities.Runtime.Abilities {
    [DisallowMultipleComponent, RequireComponent(typeof(AttributeSet), typeof(GameplayEffectCoordinator))]
    public class AbilitySystem : MonoBehaviour {
        private HashSet<IAbility> Abilities { get; } = new HashSet<IAbility>();
        private HashSet<Perk> Perks { get; } = new HashSet<Perk>();
        private Dictionary<IAbility, int> ActiveAbilities { get; } = new Dictionary<IAbility, int>();
        
        private Dictionary<GameplayEffect, IAbility> ActiveEffects { get; } =
            new Dictionary<GameplayEffect, IAbility>();
        
        private GameplayEffectCoordinator GameplayEffectCoordinator { get; set; }
        private AttributeSet AttributeSet { get; set; }
        
        [field: SerializeField] private UnityEvent<IAbility> OnStartAbility { get; set; }
        [field: SerializeField] private UnityEvent<IAbility> OnEndAbility { get; set; }

        private void Awake() {
            this.AttributeSet = this.GetComponent<AttributeSet>();
            this.GameplayEffectCoordinator = this.GetComponent<GameplayEffectCoordinator>();
        }
        
        /// <summary>
        /// Add a gameplay effect to the attribute set.
        /// </summary>
        /// <param name="effect">The gameplay effect.</param>
        /// <param name="chance">The base probability of this effect being successfully applied.</param>
        private void AddEffect(GameplayEffect effect, int chance) {
            if (effect.Commit(this.AttributeSet, chance) == GameplayEffect.Outcome.Success) {
                this.GameplayEffectCoordinator.Add(effect);
            }
        }

        /// <summary>
        /// Add a gameplay effect to the attribute set.
        /// </summary>
        /// <param name="effect">The gameplay effect.</param>
        public void AddEffect(GameplayEffectData effect) {
            GameplayEffectExecutionArgs args = this.CreateEffectExecutionArgs();
            GameplayEffect gameplayEffect = effect.Instantiate(this.AttributeSet, args);
            this.AddEffect(gameplayEffect, effect.BaseChance);
        }
        
        public void Enable(Perk perk) {
            if (!this.Perks.Add(perk)) {
                Debug.LogError($"Perk {perk} has already been enabled for {this.gameObject.name}", this);
            } else {
                foreach (Modifier modifier in perk.Modifiers.Select(m => m.ToModifier())) {
                    this.AttributeSet.AddModifier(modifier);
                }

                foreach (Ability ability in perk.Abilities) {
                    this.Grant(ability);
                }
            }
        }

        public void Disable(Perk perk) {
            if (!this.Perks.Remove(perk)) {
                Debug.LogError($"Removing perk {perk} before it is enabled for {this.gameObject.name}", this);
            } else {
                foreach (Modifier modifier in perk.Modifiers.Select(m => m.ToModifier())) {
                    this.AttributeSet.AddModifier(-modifier);
                }
                
                foreach (Ability ability in perk.Abilities) {
                    this.Revoke(ability);
                }
            }
        }

        public void Grant(IAbility ability) {
            this.Abilities.Add(ability);
        }

        public void Revoke(IAbility ability) {
            this.Abilities.Remove(ability);
        }
        
        public void Use(IAbility ability, AbilitySystem target) {
            if (!this.Abilities.Contains(ability) || !ability.IsUsable(this.AttributeSet, target.AttributeSet)) {
                return;
            }
            
            this.OnStartAbility.Invoke(ability);
            target.Process(ability);
        }

        private void Process(IAbility ability) {
            foreach (GameplayEffect effect in ability.GenerateEffects(this.CreateEffectExecutionArgs())) {
                this.AddEffect(effect, effect.Data.BaseChance);
                effect.OnEnded += () => handleEndedEffect(effect);
            }

            return;
            
            void handleEndedEffect(GameplayEffect effect) {
                if (!this.ActiveEffects.Remove(effect)) {
                    return;
                }

                this.ActiveAbilities[ability] -= 1;
                if (this.ActiveAbilities[ability] > 0) {
                    return;
                }

                this.ActiveAbilities.Remove(ability);
                this.OnEndAbility.Invoke(ability);
            }
        }

        private GameplayEffectExecutionArgs CreateEffectExecutionArgs() {
            return GameplayEffectExecutionArgs.Builder.From(this.AttributeSet).Build();
        }
    }
}
