using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;
using WeaponsSystem.Runtime.Attacks;
using WeaponsSystem.Runtime.Weapons;

namespace WeaponsSystem.Runtime.WeaponComponents {
    public class WeaponComponentController : WeaponController {
        private int Capacity { get; set; }
        private Dictionary<int, WeaponComponent> Components { get; set; } = new Dictionary<int, WeaponComponent>();

        [field: SerializeField, SaintsHashSet]
        private SaintsHashSet<WeaponComponent> PossibleComponents { get; set; } = new SaintsHashSet<WeaponComponent>(); 
        
        private event UnityAction<ISet<WeaponComponent>> OnComponentSetChanged;

        public override void Possess(Weapon weapon, AttributeSet stats) {
            base.Possess(weapon, stats);
            this.Components.Clear();
            this.Capacity = weapon.ComponentCapacity;
        }

        private void DisableAllComponents() {
            for (int i = 0; i < this.Capacity; i += 1) {
                if (this.Components.TryGetValue(this.Capacity - 1 - i, out WeaponComponent component)) {
                    component.Disable(this.Weapon, this.Stats);
                }
            }
        }

        private void EnableAllComponents() {
            for (int i = 0; i < this.Capacity; i += 1) {
                if (this.Components.TryGetValue(i, out WeaponComponent component)) {
                    component.Enable(this.Weapon, this.Stats);
                }
            }
        }
        
        public void AddComponent([NotNull] WeaponComponent component, int index) {
            if (index > this.Capacity) {
#if DEBUG
                Debug.LogError($"Index {index} is out of bounds for components list.");
#endif
                return;
            }

            if (!this.PossibleComponents.Contains(component)) {
#if DEBUG
                Debug.LogError($"Component {component.name} is not allowed");
#endif
                return;
            }

            this.RemoveComponent(index);
            this.Components[index] = component;
            this.EnableAllComponents();
        }
        
        public bool RemoveComponent(int index) {
            if (index > this.Capacity) {
#if DEBUG
                Debug.LogError($"Index {index} is out of bounds for components list.");
#endif
                return false;
            }

            if (!this.Components.ContainsKey(index)) {
                return false;
            }
            
            this.DisableAllComponents();
            this.Components.Remove(index);
            this.EnableAllComponents();
            return true;
        }

        public WeaponComponent GetComponent(int index) {
            if (index < this.Capacity) {
                return this.Components[index];
            }
#if DEBUG
            Debug.LogError($"Index {index} is out of bounds for components list.");
#endif
            return null;
        }

        public bool HasComponent(int index, out WeaponComponent component) {
            if (index <= this.Capacity) {
                component = this.Components[index];
                return component;
            }
#if DEBUG
            Debug.LogError($"Index {index} is out of bounds for components list.");
#endif
            component = null;
            return false;
        }

        public override void UpdateOnAttack(ref AttackAction action) {
            for (int i = 0; i < this.Capacity; i += 1) {
                if (this.Components.TryGetValue(i, out WeaponComponent component)) {
                    component.PreprocessAttack(this.Weapon, this.Stats);
                }
            }
        }
        
        public override void UpdatePostAttack() {
            for (int i = 0; i < this.Capacity; i += 1) {
                if (this.Components.TryGetValue(this.Capacity - i - 1, out WeaponComponent component)) {
                    component.PostprocessAttack(this.Weapon, this.Stats);
                }
            }
        }
    }
}
