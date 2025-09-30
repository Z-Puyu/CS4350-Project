using System;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;
using Weapons.Runtime;

namespace WeaponsSystem.WeaponControllers {
    public class AttributeBasedWeaponComboController : WeaponComboController {
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))] 
        private string ComboAttribute { get; set; }
        
        public override int ComboLength => this.Stats.Get(this.ComboAttribute);
        private AdvancedDropdownList<string> AttributeOptions => this.GetAttributeOptions();
    }
}
