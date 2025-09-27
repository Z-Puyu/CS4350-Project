using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataStructuresForUnity.Runtime.Bitmasks;
using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using Unity.VisualScripting;
using UnityEngine;
using WeaponsSystem.Projectiles;
using UnityEngine.Events;
using WeaponsSystem.DamageHandling;

namespace WeaponsSystem.WeaponComponent {
    [DisallowMultipleComponent]
    public sealed class ComponentManager : MonoBehaviour {
        [field: SerializeField, MinValue(1)] private int Capacity { get; set; } = 3;
        [field: SerializeField] private WeaponStats Stats { get; set; }
        
        private Dictionary<WeaponComponentData, int> PossibleComponents { get; } = new Dictionary<WeaponComponentData, int>();
        [field: SerializeField] private WeaponComponentData[] Components { get; set; }
        private List<Modifier> WeaponModifiers { get; } = new List<Modifier>();
        private List<ProjectileEffect> WeaponProjectileEffects { get; } = new List<ProjectileEffect>();
        private Dictionary<int, List<AttackData>> ComboModifiers { get; } = new Dictionary<int, List<AttackData>>();
        public Bitmask64 ComponentCombination { get; } = 0;
        
        [field: SerializeField]
        private List<AttackData> DefaultAttacks { get; set; }
        [field: SerializeField]
        private ComponentSkillTable ComponentSkillTable { get; set; }
        [field: SerializeField] private AbilitySystem AbilitySystem { get; set; }
        [field: SerializeField] private Combatant Combatant { get; set; }
        private UnityAction<string> OnSkillActivatable { get; set; }

        public void AddComponent(WeaponComponentData component, int index) {
            if (index > this.Capacity) {
#if DEBUG
                Debug.LogError($"Index {index} is out of bounds for components list.", this);
#endif
                return;
            }

            if (!component || !this.PossibleComponents.TryGetValue(component, out int id)) {
#if DEBUG
                Debug.LogError($"Component {component.Name} is not allowed", this);
#endif
                return;
            }

            this.ComponentCombination.Set(id);
            this.Components[index] = component;
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
            if (component) {
                this.ComponentCombination.Unset(this.PossibleComponents[component]);
            }

            this.Components[index] = null;
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

        private void Awake() {
            // this.Components = Enumerable.Repeat<WeaponComponentData>(null, this.Capacity).ToArray();
        }

        /*public void Start() {
            this.RefreshModifiers();
            this.ApplyComponentsToStats();
        }*/

        public void Initialise(ComponentSet components) {
            this.PossibleComponents.Clear();
            foreach (WeaponComponentData component in components.Components) {
                this.PossibleComponents.Add(component, this.PossibleComponents.Count);
            }
            
            this.OnSkillActivatable += this.AbilitySystem.Grant; 
            this.OnSkillActivatable += this.Combatant.AddUsableSkill;
            this.RefreshModifiers();
            this.ApplyDefaultAttacks();
            this.ApplyComponentsToStats();
            
            this.StartCoroutine(this.DelayedActivateSkills(2.0f));
            
        }

        private void ClearModifiers() {
            foreach (Modifier modifier in this.WeaponModifiers) {
                this.Stats.RemoveWeaponModifier(modifier);
            }

            this.WeaponModifiers.Clear();
            foreach (KeyValuePair<int, List<AttackData>> modifier in this.ComboModifiers) {
                this.Stats.RemoveAttackModifier(modifier.Key, modifier.Value);    
            }

            this.WeaponProjectileEffects.Clear();
            }
        }

        private void ApplyDefaultAttacks() {
            Dictionary<int, List<AttackData>> defaultAttacks = new Dictionary<int, List<AttackData>>();
            for (int i = 0; i < this.DefaultAttacks.Count; i += 1) {
                List<AttackData> list = new List<AttackData> { this.DefaultAttacks[i] };
                defaultAttacks.Add(i, list);
            }

            foreach (KeyValuePair<int, List<AttackData>> attack in defaultAttacks) {
                this.Stats.AddAttackModifier(attack.Key, attack.Value);
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
        }

        private void ApplyComponentsToStats() {
            foreach (Modifier modifier in this.WeaponModifiers) {
                this.Stats.AddWeaponModifier(modifier);
            }

            foreach (KeyValuePair<int, List<AttackData>> modifier in this.ComboModifiers) {
                this.Stats.AddAttackModifier(modifier.Key, modifier.Value);
            }
        }

        private void ActivateComponentSkills() {
            foreach (KeyValuePair<HashSet<WeaponComponentData>, string> entry in this.ComponentSkillTable) {
                if (entry.Key.IsSubsetOf(this.Components)) {
                    Debug.Log($"Component Manager Activating skill {entry.Value}", this);
                    this.OnSkillActivatable?.Invoke(entry.Value);
                }
            }
        }

        private IEnumerator DelayedActivateSkills(float delay) {
            yield return new WaitForSeconds(delay);
            this.ActivateComponentSkills();
        }
    }
}
