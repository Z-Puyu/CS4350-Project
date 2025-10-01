using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;
using WeaponsSystem.Weapons;

namespace WeaponsSystem.WeaponComponents {
    public class WeaponComponentController : WeaponController {
        private int Capacity { get; set; }
        private WeaponComponent[] Components { get; set; }

        [field: SerializeField, SaintsHashSet]
        private SaintsHashSet<WeaponComponent> PossibleComponents { get; set; } = new SaintsHashSet<WeaponComponent>(); 
        
        private event UnityAction<ISet<WeaponComponent>> OnComponentSetChanged;

        public override void Possess(Weapon weapon, AttributeSet stats) {
            base.Possess(weapon, stats);
            this.Components = new WeaponComponent[this.Capacity];
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

            if (this.Components[index]) {
                this.RemoveComponent(index);           
            }
            
            this.Components[index] = component;
            foreach (WeaponComponent comp in this.Components) {
                if (comp) {
                    comp.Enable(this.Weapon, this.Stats);
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
                    comp.Disable(this.Weapon, this.Stats);
                }
            }
            
            return component;
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

        public override void UpdateOnAttack() {
            foreach (WeaponComponent comp in this.Components) {
                if (comp) {
                    comp.PreprocessAttack(this.Weapon, this.Stats);
                }
            }
        }
        
        public override void UpdatePostAttack() {
            foreach (WeaponComponent comp in this.Components) {
                if (comp) {
                    comp.PostprocessAttack(this.Weapon, this.Stats);
                }
            }
        }
    }
}
