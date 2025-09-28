using System;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.DamageHandling {
    [Serializable]
    public struct DamageFeedback {
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))] 
        public string Attribute { get; private set; }
        
        [field: SerializeField] public float Rate { get; private set; }
        
        private AdvancedDropdownList<string> AttributeOptions => this.GetAttributeOptions();
    }
}
