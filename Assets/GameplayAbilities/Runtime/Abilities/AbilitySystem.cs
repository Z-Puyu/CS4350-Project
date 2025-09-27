using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
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
        
        private HashSet<IAbility> AvailableAbilities { get; } = new HashSet<IAbility>();
        private Dictionary<IAbility, double> AbilitiesOnCooldown { get; } = new Dictionary<IAbility, double>();
        private HashSet<Perk> Perks { get; } = new HashSet<Perk>();
        private Dictionary<IAbility, double> TimedAbilities { get; } = new Dictionary<IAbility, double>();
        private HashSet<IAbility> IndefiniteAbilities { get; } = new HashSet<IAbility>();
        private HashSet<IAbility> EndedIndefiniteAbilities { get; } = new HashSet<IAbility>();
        
        private GameplayEffectCoordinator GameplayEffectCoordinator { get; set; }
        private AttributeSet AttributeSet { get; set; }
        
        [field: SerializeField] private UnityEvent<AbilityData> OnStartAbility { get; set; }

        private void Awake() {
            this.AttributeSet = this.GetComponent<AttributeSet>();
            this.GameplayEffectCoordinator = this.GetComponent<GameplayEffectCoordinator>();
        }

        private void Start() {
            foreach (Ability ability in this.DefaultAbilities) {
                this.Grant(ability);
            }
        }
        
        public bool CanUse(IAbility ability) {
            return !this.AbilitiesOnCooldown.ContainsKey(ability);
        }
        
        public bool CanUse(string abilityId) {
            IAbility ability = PerkDatabase.GetAbility(abilityId);
            return this.CanUse(ability);
        }

        private DropdownList<string> GetAllAbilities() {
            DropdownList<string> list = new DropdownList<string>();
            foreach (string id in PerkDatabase.GetAllAbilityIds()) {
                list.Add(id, id);
            }
            
            return list;
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
            
            this.AvailableAbilities.Add(ability);
        }

        public void Grant(string abilityId) {
            IAbility ability = PerkDatabase.GetAbility(abilityId);
            this.Grant(ability);
            Debug.Log($"Granted ability {abilityId}", this);
        }

        public void Revoke(IAbility ability) {
            this.AvailableAbilities.Remove(ability);
        }
        
        public void Use(IAbility ability, AbilitySystem target, GameplayEffectExecutionArgs args) {
            if (!this.AvailableAbilities.Contains(ability) || !ability.IsUsable(this.AttributeSet, target.AttributeSet)) {
                return;
            }

            if (this.AbilitiesOnCooldown.ContainsKey(ability)) {
                return;
            }
            
            this.OnStartAbility.Invoke(new AbilityData(ability.Info));
            target.Process(ability, args);
        }

        public void Use(string abilityId, AbilitySystem target, GameplayEffectExecutionArgs args) {
            IAbility ability = PerkDatabase.GetAbility(abilityId);
            if (ability == null) {
                Debug.LogError($"Ability {abilityId} not found!", this);
            }
            
            this.Use(ability, target, args);
        }

        private void Process(IAbility ability, GameplayEffectExecutionArgs args) {
            AbilityInfo info = ability.Info;
            if (info.Duration > 0) {
                this.TimedAbilities.Add(ability, Time.timeAsDouble + info.Duration);
            }

            if (ability.Info.Cooldown > 0) {
                this.AbilitiesOnCooldown[ability] = Time.timeAsDouble + info.DurationPlusCooldown;
            }
            
            foreach (GameplayEffect effect in ability.GenerateEffects(args)) {
                this.GameplayEffectCoordinator.Add(effect, effect.Data.BaseChance, ability);
            }
        }

        public GameplayEffectExecutionArgs.Builder CreateEffectExecutionArgs() {
            return GameplayEffectExecutionArgs.From(this.AttributeSet, this.transform);
        }

        public void End(IAbility ability) {
            if (!this.IndefiniteAbilities.Remove(ability)) {
                return;
            }
            
            this.GameplayEffectCoordinator.End(ability);
            double cooldown = ability.Info.Cooldown;
            if (cooldown > 0) {
                this.AbilitiesOnCooldown[ability] = Time.timeAsDouble + cooldown;
            }
        }
        
        private void UpdateTimedAbilities() {
            List<IAbility> toEnd = new List<IAbility>();
            foreach (KeyValuePair<IAbility, double> ability in this.TimedAbilities) {
                if (ability.Value <= Time.timeAsDouble) {
                    toEnd.Add(ability.Key);
                }
            }
            
            foreach (IAbility ability in toEnd) {
                this.TimedAbilities.Remove(ability);
            }
        }

        private void UpdateCooldowns() {
            List<IAbility> toEnd = new List<IAbility>();
            foreach (KeyValuePair<IAbility, double> ability in this.AbilitiesOnCooldown) {
                if (ability.Value <= Time.timeAsDouble) {
                    toEnd.Add(ability.Key);
                }
            }
            
            foreach (IAbility ability in toEnd) {
                this.AbilitiesOnCooldown.Remove(ability);
            }
        }

        private void Update() {
            this.UpdateTimedAbilities();
            this.UpdateCooldowns();
        }
        
    }
}
