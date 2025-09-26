using System;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.DamageHandling {
    [Serializable]
    public struct DamageType {
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))] 
        public string DamageAttribute { get; private set; }
        
        [field: SerializeField, TableColumn("Defended by"), TreeDropdown(nameof(this.AttributeOptions))] 
        public string DefenceAttribute { get; private set; }
        
        [field: SerializeField] public bool IsPercentageDefence { get; private set; }
        [field: SerializeField, MinValue(0)] public int DefenceCoefficient { get; private set; }
        
        private AdvancedDropdownList<string> AttributeOptions => this.GetAttributeOptions();
    }
}
