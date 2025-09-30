using System;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;
using Weapons.Runtime;

namespace WeaponsSystem.WeaponComponents {
    [Serializable]
    public struct ComponentModifier {
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))]
        public string Target { get; private set; }

        [field: SerializeField] public int Magnitude { get; private set; }
        
        public WeaponStats.ModifierType Type { get; private set; }
        
        private AdvancedDropdownList<string> AttributeOptions => this.GetAttributeOptions();
    }
}
