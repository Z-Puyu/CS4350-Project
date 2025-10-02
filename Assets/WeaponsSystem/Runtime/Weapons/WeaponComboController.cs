using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;
using WeaponsSystem.Runtime.Attacks;

namespace WeaponsSystem.Runtime.Weapons {
    public class WeaponComboController : WeaponController {
        [field: SerializeField, MinValue(1f)] public float ComboResetTime { get; private set; }
        
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))]
        private string ComboLengthAttribute { get; set; }
        
        private AdvancedDropdownList<string> AttributeOptions => this.GetAttributeOptions();

        private int ComboLength => this.Stats.GetCurrent(this.ComboLengthAttribute);

        public override void UpdateOnAttack(ref AttackAction action) {
            this.Weapon.NextCombo(this.ComboLength);
        }
        
        public override void UpdatePostAttack() {
            this.Weapon.ResetComboAfter(this.ComboResetTime);
        }
    }
}
