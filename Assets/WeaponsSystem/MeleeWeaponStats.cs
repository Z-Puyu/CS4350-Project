using SaintsField;
using UnityEngine;

namespace WeaponsSystem {
    public sealed class MeleeWeaponStats : WeaponStats {
        [field: SerializeField, Required, PropRange(0, 180)] 
        public float SweepHalfAngle { get; private set; }
        
        [field: SerializeField, Required, TreeDropdown(nameof(this.AttributeOptions))] 
        public string MeleeRangeAttribute { get; private set; }
    }
}
