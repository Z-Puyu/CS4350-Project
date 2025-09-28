using SaintsField;
using UnityEngine;

namespace WeaponsSystem {
    public sealed class RangedWeaponStats : WeaponStats {
        [field: SerializeField, Required, Dropdown(nameof(this.GetAttributeOptions))]
        public string RangeAttribute { get; private set; }
        
        [field: SerializeField, Required, Dropdown(nameof(this.GetAttributeOptions))]
        public string BulletSpeedAttribute { get; private set; }
        
        [field: SerializeField, Required, Dropdown(nameof(this.GetAttributeOptions))]
        public string BulletSpreadAttribute { get; private set; }
        
        [field: SerializeField, Required, Dropdown(nameof(this.GetAttributeOptions))]
        public string FireIntervalAttribute { get; private set; }
        
        [field: SerializeField, Required, Dropdown(nameof(this.GetAttributeOptions))]
        public string PierceStrengthAttribute { get; private set; }
    }
}
