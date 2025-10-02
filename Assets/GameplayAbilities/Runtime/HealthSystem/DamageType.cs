using System;
using System.Data;
using DataStructuresForUnity.Runtime.Utilities;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.HealthSystem {
    [Serializable]
    public struct DamageType {
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))] 
        public string DamageAttribute { get; private set; }
        
        [field: SerializeField, MinValue(0)] private int DefaultValue { get; set; }
        
        [field: SerializeField, TableColumn("Defended by"), TreeDropdown(nameof(this.AttributeOptions))] 
        public string DefenceAttribute { get; private set; }
        
        [field: SerializeField] public bool IsPercentageDefence { get; private set; }
        [field: SerializeField, MinValue(0)] public int DefenceCoefficient { get; private set; }
        
        private AdvancedDropdownList<string> AttributeOptions => this.GetAttributeOptions();
        
        public Modifier ToModifier(IDataReader<string, int> target, string targetAttribute) {
            int rawDamage = this.DefaultValue;
            if (target.HasValue(this.DamageAttribute, out int damageValue) && damageValue > 0) {
                rawDamage = damageValue;
            }
            
            if (rawDamage <= 0) {
                return Modifier.Empty;
            }
            
            int defence = target.HasValue(this.DefenceAttribute, out int defenceValue) ? defenceValue : 0;
            int damage = this.IsPercentageDefence
                    ? Mathf.RoundToInt(Math.Max(0, 100 - defence) * rawDamage / 100f)
                    : rawDamage - defence;
            return new Modifier(-damage, Modifier.Operation.Offset, targetAttribute);
        }
    }
}
