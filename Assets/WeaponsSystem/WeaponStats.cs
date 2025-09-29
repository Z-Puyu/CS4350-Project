using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Modifiers;
using GameplayEffects.Runtime;
using SaintsField;
using UnityEngine;
using WeaponsSystem.Projectiles;
using Attribute = GameplayAbilities.Runtime.Attributes.Attribute;

namespace WeaponsSystem {
    [DisallowMultipleComponent, RequireComponent(typeof(AttributeSet))]
    public abstract class WeaponStats : MonoBehaviour, IDataReader<string, int> {
        private AttributeSet AttributeSet { get; set; }
        
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))] 
        private List<string> DamageAttributes { get; set; } = new List<string>();
        
        protected Dictionary<int, List<AttributeBasedAttack>> AttackModifiers { get; } =
            new Dictionary<int, List<AttributeBasedAttack>>();

        private Dictionary<int, List<Modifier>> ActiveAttackModifiers { get; } =
            new Dictionary<int, List<Modifier>>();
        
        [field: SerializeField, Required, TreeDropdown(nameof(this.AttributeOptions))]
        public string ComboLengthAttribute { get; private set; }
        
        [field: SerializeField, Required, TreeDropdown(nameof(this.AttributeOptions))] 
        public string KnockbackStrengthAttribute { get; private set; }
        
        public List<ProjectileEffect> ProjectileEffects { get; } = new List<ProjectileEffect>();
        public ProjectileSpawner.Mode ProjectileMode { get; protected set; } = ProjectileSpawner.Mode.None;

        protected AdvancedDropdownList<string> AttributeOptions => this.GetAttributeOptions();
        
        protected virtual void Awake() {
            this.AttributeSet = this.GetComponent<AttributeSet>();
        }

        public IReadOnlyDictionary<string, int> ReadDamageData() {
            Dictionary<string, int> damages = new Dictionary<string, int>();
            foreach (string attribute in this.DamageAttributes) {
                damages.Add(attribute, this.GetCurrent(attribute));
            }
            
            return new ReadOnlyDictionary<string, int>(damages);
        }
        
        protected abstract void UpdateProjectileMode(int index);
        
        protected abstract void RevertProjectileMode(int index);

        public void ActivateAttackModifiers(int index) {
            if (!this.AttackModifiers.TryGetValue(index, out List<AttributeBasedAttack> list)) {
                list = new List<AttributeBasedAttack>();
                this.AttackModifiers.Add(index, list);
            }
            
            if (!this.ActiveAttackModifiers.TryGetValue(index, out List<Modifier> current)) {
                current = new List<Modifier>();
                this.ActiveAttackModifiers.Add(index, current);
            }
            
            IEnumerable<Modifier> modifiers = list.SelectMany(modifier => modifier.GenerateModifiers(this));
            foreach (Modifier modifier in modifiers) {
                current.Add(modifier);
                this.AttributeSet.AddModifier(modifier);
            } 
            
            this.ProjectileMode = list.Count == 0 ? ProjectileSpawner.Mode.None : list.Last().ProjectileMode;
            foreach (AttributeBasedAttack data in list) {
                this.ProjectileEffects.AddRange(data.ProjectileEffects);
            }
            
            this.UpdateProjectileMode(index);
        }
        
        public void DeactivateAttackModifiers(int index) {
            foreach (Modifier modifier in this.ActiveAttackModifiers[index]) {
                this.AttributeSet.RemoveModifier(modifier);
            }
                
            this.ActiveAttackModifiers[index].Clear();
            this.ProjectileEffects.Clear();
            this.RevertProjectileMode(index);
        }

        public void Initialise(WeaponData data) {
            this.AttributeSet.Initialise(data.WeaponAttributes);
        }

        public void AddWeaponModifier(Modifier modifier) {
            this.AttributeSet.AddModifier(modifier);
        }

        public void RemoveWeaponModifier(Modifier modifier) {
            this.AttributeSet.RemoveModifier(modifier);
        }

        public void AddWeaponProjectileEffect(ProjectileEffect effect) {
            this.ProjectileEffects.Add(effect);
        }
        
        public void RemoveWeaponProjectileEffect(ProjectileEffect effect) {
            this.ProjectileEffects.Remove(effect);
        }

        public void AddAttackModifier(int index, IEnumerable<AttributeBasedAttack> modifiers) {
            if (!this.AttackModifiers.TryGetValue(index, out List<AttributeBasedAttack> list)) {
                this.AttackModifiers.Add(index, modifiers.ToList());
            } else {
                list.AddRange(modifiers);
            }
        }

        public void RemoveAttackModifier(int index, ICollection<AttributeBasedAttack> removed) {
            if (!this.AttackModifiers.ContainsKey(index)) {
                return;
            }

            this.AttackModifiers[index].RemoveAll(removed.Contains);
        }

        public void AddModifier(Modifier modifier) {
            this.AttributeSet.AddModifier(modifier);
        }
        
        public void RemoveModifier(Modifier modifier) {
            this.AttributeSet.RemoveModifier(modifier);
        }

        bool IDataReader<string, int>.HasValue(string key, out int value) {
            value = this.AttributeSet.GetCurrent(key);
            return this.AttributeSet.Has(value, key);
        }
        
        IDataReader<string, int> IDataReader<string, int>.With(string key, int value) {
            this.AttributeSet.Set(key, value);
            return this;
        }
    }
}
