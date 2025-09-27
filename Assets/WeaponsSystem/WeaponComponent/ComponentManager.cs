using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataStructuresForUnity.Runtime.Bitmasks;
using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using UnityEngine;
using WeaponsSystem.Projectiles;
using UnityEngine.Events;
using WeaponsSystem.DamageHandling;

namespace WeaponsSystem.WeaponComponent {
    [DisallowMultipleComponent]
    public sealed class ComponentManager : MonoBehaviour {
        [field: SerializeField, MinValue(1)] private int Capacity { get; set; } = 3;
        [field: SerializeField] private WeaponStats Stats { get; set; }
        private List<AttackData> DefaultAttacks { get; } = new List<AttackData>();

        [field: SerializeField]
        private List<WeaponComponentData> TestComponents { get; set; } = new List<WeaponComponentData>();
        
        private WeaponComponentData[] Components { get; set; }
        private HashSet<WeaponComponentData> PossibleComponents { get; } = new HashSet<WeaponComponentData>();
        private List<Modifier> WeaponModifiers { get; } = new List<Modifier>();
        private List<ProjectileEffect> WeaponProjectileEffects { get; } = new List<ProjectileEffect>();
        private Dictionary<int, List<AttackData>> ComboModifiers { get; } = new Dictionary<int, List<AttackData>>();
        private event UnityAction<ISet<WeaponComponentData>> OnComponentSetChanged;

        private void Awake() {
            this.Components = new WeaponComponentData[this.Capacity];
        }

        public void AddComponent(WeaponComponentData component, int index) {
            if (index > this.Capacity) {
#if DEBUG
                Debug.LogError($"Index {index} is out of bounds for components list.", this);
#endif
                return;
            }

            if (!component || !this.PossibleComponents.Contains(component)) {
#if DEBUG
                Debug.LogError($"Component {component.Name} is not allowed", this);
#endif
                return;
            }
            
            this.Components[index] = component;
            this.OnComponentSetChanged?.Invoke(this.Components.Distinct().Where(c => c).ToHashSet());
            this.RefreshModifiers();
        }

        public WeaponComponentData RemoveComponent(int index) {
            if (index > this.Capacity) {
#if DEBUG
                Debug.LogError($"Index {index} is out of bounds for components list.", this);
#endif
                return null;
            }

            WeaponComponentData component = this.Components[index];
            this.Components[index] = null;
            this.OnComponentSetChanged?.Invoke(this.Components.Distinct().Where(c => c).ToHashSet());
            this.RefreshModifiers();
            return component;
        }

        public WeaponComponentData GetComponent(int index) {
            if (index <= this.Capacity) {
                return this.Components[index];
            }
#if DEBUG
            Debug.LogError($"Index {index} is out of bounds for components list.", this);
#endif
            return null;
        }

        public bool HasComponent(int index, out WeaponComponentData component) {
            if (index <= this.Capacity) {
                component = this.Components[index];
                return component;
            }
#if DEBUG
            Debug.LogError($"Index {index} is out of bounds for components list.", this);
#endif
            component = null;
            return false;
        }

        private void ResetDefaults() {
            this.OnComponentSetChanged = null;
            this.PossibleComponents.Clear();
            this.Components = new WeaponComponentData[this.Capacity];
            this.DefaultAttacks.Clear();
            this.WeaponModifiers.Clear();
            this.ComboModifiers.Clear();
            this.WeaponProjectileEffects.Clear();
        }

        public void Initialise(Combatant combatant, WeaponData data) {
            this.ResetDefaults();
            this.OnComponentSetChanged += combatant.HandleComponentSetChange;
            foreach (WeaponComponentData component in data.PossibleComponents) {
                this.PossibleComponents.Add(component);
            }

#if DEBUG
            foreach (WeaponComponentData component in this.TestComponents) {
                this.AddComponent(component, this.TestComponents.IndexOf(component));
            }
#endif
            
            this.DefaultAttacks.AddRange(data.DefaultAttacks);
            this.ApplyDefaultAttacks();
            this.RefreshModifiers();
        }

        private void ClearModifiers() {
            foreach (Modifier modifier in this.WeaponModifiers) {
                this.Stats.RemoveWeaponModifier(modifier);
            }

            this.WeaponModifiers.Clear();
            foreach (KeyValuePair<int, List<AttackData>> modifier in this.ComboModifiers) {
                this.Stats.RemoveAttackModifier(modifier.Key, modifier.Value);    
            }

            this.ComboModifiers.Clear();
            this.WeaponProjectileEffects.Clear();
        }

        private void ApplyDefaultAttacks() {
            for (int i = 0; i < this.DefaultAttacks.Count; i += 1) {
                this.Stats.AddAttackModifier(i, new List<AttackData> { this.DefaultAttacks[i] });
            }
        }

        private void RefreshModifiers() {
            this.ClearModifiers();
            foreach (WeaponComponentData component in this.Components) {
                if (!component) {
                    continue;
                }
                
                foreach (ModifierData data in component.WeaponModifiers) {
                    this.WeaponModifiers.Add(data.CreateModifier(this.Stats));
                }

                foreach (ProjectileEffect effect in component.ProjectileEffects) {
                    this.WeaponProjectileEffects.Add(effect);
                }

                for (int i = 0; i < component.AttackData.Count; i += 1) {
                    AttackData data = component.AttackData[i];
                    if (data is null || data.IsEmpty) {
                        continue;
                    }
                    
                    if (this.ComboModifiers.TryGetValue(i, out List<AttackData> list)) {
                        list.Add(data);
                    } else {
                        this.ComboModifiers.Add(i, new List<AttackData> { data });
                    }
                }
            }
            
            this.ApplyComponentsToStats();
        }

        private void ApplyComponentsToStats() {
            foreach (Modifier modifier in this.WeaponModifiers) {
                this.Stats.AddWeaponModifier(modifier);
            }

            foreach (KeyValuePair<int, List<AttackData>> modifier in this.ComboModifiers) {
                this.Stats.AddAttackModifier(modifier.Key, modifier.Value);
            }
        }

        /*private void ActivateComponentSkills() {
            foreach (KeyValuePair<ISet<WeaponComponentData>, string> entry in this.ComponentSkillTable) {
                if (!entry.Key.IsSubsetOf(this.Components)) {
                    continue;
                }

                Debug.Log($"Component Manager Activating skill {entry.Value}", this);
                this.OnComponentSetChanged?.Invoke(entry.Key);
            }
        }

        private IEnumerator DelayedActivateSkills(float delay) {
            yield return new WaitForSeconds(delay);
            this.ActivateComponentSkills();
        }*/
    }
}
