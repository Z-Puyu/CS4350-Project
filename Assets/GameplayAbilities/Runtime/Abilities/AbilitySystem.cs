using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilities.Runtime.Abilities {
    [DisallowMultipleComponent, RequireComponent(typeof(AttributeSet), typeof(GameplayEffectCoordinator))]
    public class AbilitySystem : MonoBehaviour {
        [field: SerializeField, SaintsHashSet] 
        private SaintsHashSet<Ability> DefaultAbilities { get; set; } = new SaintsHashSet<Ability>();
        
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

        private void Start() {
            foreach (Ability ability in this.DefaultAbilities) {
                this.Grant(ability);
            }
        }

        private DropdownList<string> GetAllAbilities() {
            DropdownList<string> list = new DropdownList<string>();
            foreach (string id in PerkDatabase.GetAllAbilityIds()) {
                list.Add(id, id);
            }
            
            return list;
        }
        
        /// <summary>
        /// Add a gameplay effect to the attribute set.
        /// </summary>
        /// <param name="effect">The gameplay effect.</param>
        /// <param name="chance">The base probability of this effect being successfully applied.</param>
        public void AddEffect(GameplayEffect effect, int chance) {
            if (effect.Commit(this.AttributeSet, chance) == GameplayEffect.Outcome.Success) {
                this.GameplayEffectCoordinator.Add(effect);
            }
        }

        /// <summary>
        /// Add a gameplay effect to the attribute set.
        /// </summary>
        /// <param name="effect">The gameplay effect.</param>
        public void AddEffect(GameplayEffectData effect) {
            GameplayEffectExecutionArgs args = this.CreateEffectExecutionArgs().Build();
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
                disable(perk);
            }

            return;
            void disable(Perk p) {
                foreach (Perk child in PerkDatabase.GetChildren(p)) {
                    disable(child);
                }

                if (!this.Perks.Contains(p)) {
                    return;
                }
                
                foreach (Modifier modifier in p.Modifiers.Select(m => m.ToModifier())) {
                    this.AttributeSet.AddModifier(-modifier);
                }
                
                foreach (Ability ability in p.Abilities) {
                    this.Revoke(ability);
                }
            }
        }

        public void Grant(IAbility ability) {
            if (ability == null) {
                return;
            }
            
            this.Abilities.Add(ability);
        }

        public void Revoke(IAbility ability) {
            this.Abilities.Remove(ability);
        }
        
        public void Use(IAbility ability, AbilitySystem target, GameplayEffectExecutionArgs args) {
            if (!this.Abilities.Contains(ability) || !ability.IsUsable(this.AttributeSet, target.AttributeSet)) {
                return;
            }
            
            this.OnStartAbility.Invoke(ability);
            target.Process(ability, args);
        }

        private void Process(IAbility ability, GameplayEffectExecutionArgs args) {
            foreach (GameplayEffect effect in ability.GenerateEffects(args)) {
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

        public GameplayEffectExecutionArgs.Builder CreateEffectExecutionArgs() {
            return GameplayEffectExecutionArgs.From(this.AttributeSet);
        }
    }
}
