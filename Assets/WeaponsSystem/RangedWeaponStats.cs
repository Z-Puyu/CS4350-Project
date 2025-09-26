using SaintsField;
using UnityEngine;

namespace WeaponsSystem {
    public sealed class RangedWeaponStats : WeaponStats {
        [field: SerializeField, Required, TreeDropdown(nameof(this.AttributeOptions))]
        public string ProjectileSpreadAttribute { get; private set; }
        
        [field: SerializeField, Required, TreeDropdown(nameof(this.AttributeOptions))]
        public string FireIntervalAttribute { get; private set; }
        
        [field: SerializeField, Required, TreeDropdown(nameof(this.AttributeOptions))]
        public string ShotsPerAttackAttribute { get; private set; }
        
        [field: SerializeField, Required, TreeDropdown(nameof(this.AttributeOptions))]
        public string ProjectilesPerShotAttribute { get; private set; }
        
        [field: SerializeField, Required, TreeDropdown(nameof(this.AttributeOptions))]
        public string ExplosionRadiusAttribute { get; private set; }
        
        [field: SerializeField, Required, TreeDropdown(nameof(this.AttributeOptions))]
        public string ParallelProjectileSpacingAttribute { get; private set; }
    }
}
