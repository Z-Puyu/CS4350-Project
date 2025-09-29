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
    [DisallowMultipleComponent, RequireComponent(typeof(AttributeSet))]
    public class AbilitySystem : MonoBehaviour {
        public HashSet<IAbility> AvailableAbilities { get; } = new HashSet<IAbility>();
        private Dictionary<IAbility, double> AbilitiesOnCooldown { get; } = new Dictionary<IAbility, double>();
        private HashSet<Perk> Perks { get; } = new HashSet<Perk>();
        public AttributeSet AttributeSet { get; set; }

        private void Awake() {
            this.AttributeSet = this.GetComponent<AttributeSet>();
        }
        
        public bool CanUse(IAbility ability) {
            return !this.AbilitiesOnCooldown.ContainsKey(ability);
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
#if DEBUG
            Debug.Log($"Granted ability {ability}", this);
#endif
            this.AvailableAbilities.Add(ability);
        }

        public void Revoke(IAbility ability) {
            this.AvailableAbilities.Remove(ability);
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
            this.UpdateCooldowns();
        }
    }
}
