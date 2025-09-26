using System.Collections.Generic;
using System.Linq;
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
        
        public AttackMode FireMode { get; private set; } = AttackMode.Default;
        
        public override List<AttackData> ActivateAttackModifiers(int index) {
            List<AttackData> modifiers = base.ActivateAttackModifiers(index);
            this.FireMode = modifiers.Count == 0 ? AttackMode.Default : modifiers.Last().Mode;
            return modifiers;
        }

        public override List<AttackData> DeactivateAttackModifiers(int index) {
            this.FireMode = AttackMode.Default;
            return base.DeactivateAttackModifiers(index);
        }
    }
}
