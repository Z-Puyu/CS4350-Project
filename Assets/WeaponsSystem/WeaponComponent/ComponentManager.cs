using System;
using System.Collections.Generic;
using GameplayAbilities.Runtime.Modifiers;
using UnityEngine;

namespace WeaponsSystem.WeaponComponent {
    public class ComponentManager : MonoBehaviour {
        [field: SerializeField]
        private WeaponStats Stats { get; set; }
        [field: SerializeField]
        private List<WeaponComponentData> Components { get; set; }
        private List<Modifier> WeaponModifiers { get; set; } = new List<Modifier>();
        private List<(int, Modifier)> ComboModifiers { get; set; } = new List<(int, Modifier)>();

        protected void AddComponent(WeaponComponentData component, int index) {
            if (index > 3) {
                Debug.LogError($"Index {index} is out of bounds for components list.", this);
                return;
            }

            this.Components.Insert(index, component);
        }
        
        protected WeaponComponentData RemoveComponent(int index) {
            if (index > 3) {
                Debug.LogError($"Index {index} is out of bounds for components list.", this);
                return null;
            }

            WeaponComponentData component = this.Components[index];
            this.Components.RemoveAt(index);
            return component;
        }
        
        protected WeaponComponentData GetComponent(int index) {
            if (index > 3) {
                Debug.LogError($"Index {index} is out of bounds for components list.", this);
                return null;
            }

            return this.Components[index];
        }

        public void Awake() {
            
        }
        
        public void Start() {
            this.UpdateModifiers();
            this.ApplyComponentsToStats();
        }

        private void UpdateModifiers() {
            foreach (WeaponComponentData component in this.Components) {
                foreach (ComponentModifierData modifierData in component.modifiers) {
                    if (modifierData.ModifyCombo()) {
                        this.ComboModifiers.Add((modifierData.ComboIndex, new Modifier(modifierData.ModifierData.Magnitude, modifierData.ModifierData.Type, modifierData.ModifierData.Target.Id)));
                        Debug.Log($"Added combo modifier {modifierData.ModifierData.Magnitude} to {modifierData.ModifierData.Target.Id} for combo {modifierData.ComboIndex}", this);
                    } else {
                        this.WeaponModifiers.Add(new Modifier(
                            modifierData.ModifierData.Magnitude, modifierData.ModifierData.Type, modifierData.ModifierData.Target.Id));
                        Debug.Log($"Added weapon modifier {modifierData.ModifierData.Magnitude} to {modifierData.ModifierData.Target.Id}", this);
                    }
                }
            }
        }
        
        private void ApplyComponentsToStats() {
            foreach (Modifier modifier in this.WeaponModifiers) {
                this.Stats.AddWeaponModifier(modifier);
            }

            foreach ((int, Modifier) comboModifier in this.ComboModifiers) {
                this.Stats.AddAttackModifier(comboModifier.Item2, comboModifier.Item1);
            }
        }
        
    }
}
