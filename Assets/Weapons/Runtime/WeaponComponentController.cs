using System;
using System.Collections.Generic;
using System.Linq;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace Weapons.Runtime {
    public class WeaponComponentController : WeaponController {
        private Weapon Owner { get; set; }
        private int Capacity { get; set; }
        private WeaponComponent[] Components { get; set; }
        private HashSet<WeaponComponent> PossibleComponents { get; } = new HashSet<WeaponComponent>();
        private event UnityAction<ISet<WeaponComponent>> OnComponentSetChanged;

        public WeaponComponentController(
            Weapon weapon, IEnumerable<WeaponComponent> possibleComponents, int capacity = 3
        ) {
            this.Owner = weapon;
            this.Capacity = capacity;
            this.PossibleComponents.Clear();
            foreach (WeaponComponent component in possibleComponents) {
                this.PossibleComponents.Add(component);
            }

            this.Components = new WeaponComponent[this.Capacity];
        }

        public void AddComponent(WeaponComponent component, int index) {
            if (index > this.Capacity) {
#if DEBUG
                Debug.LogError($"Index {index} is out of bounds for components list.");
#endif
                return;
            }
            
            if (!component) {
#if DEBUG
                Debug.LogError("Component is null");
#endif
                return;
            }

            if (!this.PossibleComponents.Contains(component)) {
#if DEBUG
                Debug.LogError($"Component {component.name} is not allowed");
#endif
                return;
            }

            if (this.Components[index]) {
                this.RemoveComponent(index);           
            }
            
            this.Components[index] = component;
            foreach (WeaponComponent comp in this.Components) {
                if (comp) {
                    comp.Modify(this.Owner, this.Stats);
                }
            }
        }
        
        public WeaponComponent RemoveComponent(int index) {
            if (index > this.Capacity) {
#if DEBUG
                Debug.LogError($"Index {index} is out of bounds for components list.");
#endif
                return null;
            }

            WeaponComponent component = this.Components[index];
            this.Components[index] = null;
            foreach (WeaponComponent comp in this.Components) {
                if (comp) {
                    comp.UndoEffects(this.Owner, this.Stats);
                }
            }
            
            return component;
        }

        public WeaponComponent GetComponent(int index) {
            if (index <= this.Capacity) {
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
        
        public override float UpdateOnAttack(AttackAction action) {
            return 0;
        }
    }
}
