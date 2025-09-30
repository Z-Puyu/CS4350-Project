using System;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.Attacks {
    [Serializable]
    public struct AttackAttribute {
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))] 
        public string Id { get; private set; }
        
        [field: SerializeField] public float Coefficient { get; private set; }
        
        private AdvancedDropdownList<string> AttributeOptions => this.GetAttributeOptions();
    }
}
