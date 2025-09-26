using System.Collections.Generic;
using System.Linq;
using DataStructuresForUnity.Runtime.Bitmasks;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.WeaponComponent {
    [DisallowMultipleComponent]
    public sealed class ComponentManager : MonoBehaviour {
        [field: SerializeField, MinValue(1)] private int Capacity { get; set; } = 3;
        [field: SerializeField] private WeaponStats Stats { get; set; }
        
        private Dictionary<WeaponComponentData, int> PossibleComponents { get; } = new Dictionary<WeaponComponentData, int>();
        private WeaponComponentData[] Components { get; set; }
        private List<Modifier> WeaponModifiers { get; set; } = new List<Modifier>();
        private Dictionary<int, List<AttackData>> ComboModifiers { get; set; } = new Dictionary<int, List<AttackData>>();
        public Bitmask64 ComponentCombination { get; } = 0;

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
            this.Components = Enumerable.Repeat<WeaponComponentData>(null, this.Capacity).ToArray();
        }

        public void Start() {
            this.RefreshModifiers();
            this.ApplyComponentsToStats();
        }

        public void Initialise(ComponentSet components) {
            this.PossibleComponents.Clear();
            foreach (WeaponComponentData component in components.Components) {
                this.PossibleComponents.Add(component, this.PossibleComponents.Count);
            }
        }

        private void RefreshModifiers() {
            foreach (Modifier modifier in this.WeaponModifiers) {
                this.Stats.RemoveWeaponModifier(modifier);
            }
            
            this.WeaponModifiers.Clear();
            foreach (KeyValuePair<int, List<AttackData>> modifier in this.ComboModifiers) {
                this.Stats.RemoveAttackModifier(modifier.Key, modifier.Value);    
            }
            
            foreach (WeaponComponentData component in this.Components) {
                foreach (ModifierData data in component.WeaponModifiers) {
                    this.WeaponModifiers.Add(data.CreateModifier(this.Stats));
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

        public void ApplyComponentsToStats() {
            foreach (Modifier modifier in this.WeaponModifiers) {
                this.Stats.AddWeaponModifier(modifier);
            }

            foreach (KeyValuePair<int, List<AttackData>> modifier in this.ComboModifiers) {
                this.Stats.AddAttackModifier(modifier.Key, modifier.Value);
            }
        }
    }
}
