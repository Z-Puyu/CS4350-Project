using System.Collections.Generic;
using System.Linq;
using SaintsField;
using UnityEngine;
using WeaponsSystem.Projectiles;

namespace WeaponsSystem {
    public sealed class RangedWeaponStats : WeaponStats {
        [field: SerializeField, Required, TreeDropdown(nameof(this.AttributeOptions))]
        public string FireIntervalAttribute { get; private set; }
        
        [field: SerializeField, Required, TreeDropdown(nameof(this.AttributeOptions))]
        public string ShotsPerAttackAttribute { get; private set; }
        
        public ProjectileSpawner.Mode FireMode { get; private set; } = ProjectileSpawner.Mode.Single;
        
        public override List<AttackData> ActivateAttackModifiers(int index) {
            List<AttackData> modifiers = base.ActivateAttackModifiers(index);
            this.FireMode = modifiers.Count == 0 ? ProjectileSpawner.Mode.Single : modifiers.Last().Mode;
            return modifiers;
        }

        public override List<AttackData> DeactivateAttackModifiers(int index) {
            this.FireMode = ProjectileSpawner.Mode.Single;
            return base.DeactivateAttackModifiers(index);
        }
    }
}
