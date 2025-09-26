using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using UnityEngine;
using Attribute = GameplayAbilities.Runtime.Attributes.Attribute;

namespace WeaponsSystem {
    [DisallowMultipleComponent, RequireComponent(typeof(AttributeSet))]
    public abstract class WeaponStats : MonoBehaviour, IAttributeReader {
        private AttributeSet AttributeSet { get; set; }
        
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))] 
        private List<string> DamageAttributes { get; set; } = new List<string>();
        
        private Dictionary<int, List<AttackData>> AttackModifiers { get; set; } =
            new Dictionary<int, List<AttackData>>();

        private Dictionary<int, List<Modifier>> ActiveAttackModifiers { get; set; } =
            new Dictionary<int, List<Modifier>>();
        
        [field: SerializeField, Required, TreeDropdown(nameof(this.AttributeOptions))]
        public string ComboLengthAttribute { get; private set; }
        
        [field: SerializeField, Required, TreeDropdown(nameof(this.AttributeOptions))] 
        public string KnockbackStrengthAttribute { get; private set; }

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

        public void ActivateAttackModifiers(int index) {
            if (!this.AttackModifiers.TryGetValue(index, out List<AttackData> list)) {
                list = new List<AttackData>();
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
        }
        
        public void DeactivateAttackModifiers(int index) {
            foreach (Modifier modifier in this.ActiveAttackModifiers[index]) {
                this.AttributeSet.RemoveModifier(modifier);
            }
                
            this.ActiveAttackModifiers[index].Clear();
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

        public void AddAttackModifier(int index, IEnumerable<AttackData> modifiers) {
            if (!this.AttackModifiers.TryGetValue(index, out List<AttackData> list)) {
                this.AttackModifiers.Add(index, modifiers.ToList());
            } else {
                list.AddRange(modifiers);
            }
        }

        public void RemoveAttackModifier(int index, ICollection<AttackData> removed) {
            if (!this.AttackModifiers.ContainsKey(index)) {
                return;
            }
            
            this.AttackModifiers[index].RemoveAll(removed.Contains);
        }

        public IEnumerator<Attribute> GetEnumerator() {
            return this.AttributeSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
        
        public int GetCurrent(string key) {
            return this.AttributeSet.GetCurrent(key);
        }
        
        public int GetMax(string key) {
            return this.AttributeSet.GetMax(key);
        }
        
        public int GetMin(string key) {
            return this.AttributeSet.GetMin(key);
        }
        
        public Attribute GetAttribute(string key) {
            return this.AttributeSet.GetAttribute(key);
        }
        
        public bool Has(int threshold, string key) {
            return this.AttributeSet.Has(threshold, key);
        }
    }
}
