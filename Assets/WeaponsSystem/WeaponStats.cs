using System.Collections;
using System.Collections.Generic;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using UnityEngine;
using Attribute = GameplayAbilities.Runtime.Attributes.Attribute;

namespace WeaponsSystem {
    [DisallowMultipleComponent, RequireComponent(typeof(AttributeSet))]
    public abstract class WeaponStats : MonoBehaviour, IAttributeReader {
        [field: SerializeField] private AttributeSet AttributeSet { get; set; }
        [field: SerializeField] private List<AttackData> AttackModifiers { get; set; } = new List<AttackData>();
        
        [field: SerializeField, Required]
        private AttributeTypeDefinition ComboLengthAttributeType { get; set; }
        
        public string ComboLengthAttribute => this.ComboLengthAttributeType.Id;
        
        [field: SerializeField, Required] 
        private AttributeTypeDefinition KnockbackStrengthAttributeType { get; set; }
        
        public string KnockbackStrengthAttribute => this.KnockbackStrengthAttributeType.Id;
        
        protected virtual void Awake() {
            this.AttributeSet = this.GetComponent<AttributeSet>();
            this.AttackModifiers.ForEach(attack => attack.Initialise());
        }
        
        private void HandleAttributeChange(AttributeChange change) {
            if (change.AttributeName != this.ComboLengthAttribute) {
                return;
            }

            while (this.AttackModifiers.Count < change.CurrentValue) {
                this.AttackModifiers.Add(new AttackData());
            }
        }

        public void ActivateAttackModifiers(int index) {
            if (this.AttackModifiers.Count > index) {
                foreach (Modifier modifier in this.AttackModifiers[index].WeaponModifiers) {
                    this.AttributeSet.AddModifier(modifier);
                }
            } else {
                Debug.LogError($"Index {index} is out of bounds for attack modifiers list.", this);
            }
        }
        
        public void DeactivateAttackModifiers(int index) {
            if (this.AttackModifiers.Count > index) {
                foreach (Modifier modifier in this.AttackModifiers[index].WeaponModifiers) {
                    this.AttributeSet.AddModifier(-modifier);
                }
            } else {
                Debug.LogError($"Index {index} is out of bounds for attack modifiers list.", this);
            }
        }

        public void Initialise(WeaponData data) {
            this.AttributeSet.OnAttributeChanged += this.HandleAttributeChange;
            this.AttributeSet.Initialise(data.WeaponAttributes);
        }

        public void AddWeaponModifier(Modifier modifier) {
            this.AttributeSet.AddModifier(modifier);
        }

        public void AddAttackModifier(Modifier modifier, int index) {
            if (this.AttackModifiers.Count > index) {
                this.AttackModifiers[index].AddModifier(modifier);
            } else {
                Debug.LogError($"Index {index} is out of bounds for attack modifiers list.", this);
            }
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
