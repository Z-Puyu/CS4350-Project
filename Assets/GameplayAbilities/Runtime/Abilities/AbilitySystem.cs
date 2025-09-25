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
        private Dictionary<IAbility, float> AbilitiesOnCooldown { get; } = new Dictionary<IAbility, float>();
        private HashSet<Perk> Perks { get; } = new HashSet<Perk>();
        private Dictionary<IAbility, AbilityInfo> ActiveAbilities { get; } = new Dictionary<IAbility, AbilityInfo>();
        
        private GameplayEffectCoordinator GameplayEffectCoordinator { get; set; }
        private AttributeSet AttributeSet { get; set; }
        
        [field: SerializeField] private UnityEvent<IAbility> OnStartAbility { get; set; }
        [field: SerializeField] private UnityEvent<IAbility> OnEndAbility { get; set; }

        private void Awake() {
            this.AttributeSet = this.GetComponent<AttributeSet>();
            this.GameplayEffectCoordinator = this.GetComponent<GameplayEffectCoordinator>();
        }

        private void Start() {
            this.GameplayEffectCoordinator.OnEffectEnded += this.HandleTerminatedEffect;
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

        private void HandleTerminatedEffect(GameplayEffect effect, IAbility ability) {
            if (ability == null) {
                return;
            }
            
            AbilityInfo info = this.ActiveAbilities[ability];
            int remainingEffects = info.NumberOfEffects - 1;
            if (remainingEffects > 0) {
                this.ActiveAbilities[ability] = new AbilityInfo(info.Cooldown, remainingEffects);
            } else {
                this.ActiveAbilities.Remove(ability);
                this.OnEndAbility.Invoke(ability);
                if (info.Cooldown > 0) {
                    this.AbilitiesOnCooldown[ability] = info.Cooldown;
                }
            }
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

        public void Revoke(IAbility ability) {
            this.AvailableAbilities.Remove(ability);
        }
        
        public void Use(IAbility ability, AbilitySystem target, GameplayEffectExecutionArgs args) {
            if (!this.AvailableAbilities.Contains(ability) || !ability.IsUsable(this.AttributeSet, target.AttributeSet)) {
                return;
            }
            
            ability.StartAbility(args.InstigatorTransform.position);
            this.OnStartAbility.Invoke(ability);
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
            this.ActiveAbilities.Add(ability, ability.Info);
            if (ability.Info.Cooldown > 0) {
                this.AbilitiesOnCooldown[ability] = ability.Info.Cooldown;
            }
            
            foreach (GameplayEffect effect in ability.GenerateEffects(args)) {
                this.GameplayEffectCoordinator.Add(effect, effect.Data.BaseChance, ability);
            }
        }

        public GameplayEffectExecutionArgs.Builder CreateEffectExecutionArgs() {
            return GameplayEffectExecutionArgs.From(this.AttributeSet, this.transform);
        }

        private void Update() {
            foreach (IAbility ability in this.AbilitiesOnCooldown.Keys) {
                if (this.ActiveAbilities.ContainsKey(ability)) {
                    continue;
                }

                this.AbilitiesOnCooldown[ability] -= Time.deltaTime;
                if (this.AbilitiesOnCooldown[ability] <= 0) {
                    this.AbilitiesOnCooldown.Remove(ability);
                }
            }
        }
    }
}
