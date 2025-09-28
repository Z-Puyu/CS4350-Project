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

        protected override void Awake() {
            base.Awake();
            this.ProjectileMode = ProjectileSpawner.Mode.Single;
        }

        protected override void UpdateProjectileMode(int index) {
            List<AttackData> modifiers = this.AttackModifiers[index];
            this.ProjectileMode = modifiers.Count == 0 || modifiers.Last().ProjectileMode == ProjectileSpawner.Mode.None
                    ? ProjectileSpawner.Mode.Single
                    : modifiers.Last().ProjectileMode;
        }
        
        protected override void RevertProjectileMode(int index) {
            this.ProjectileMode = ProjectileSpawner.Mode.Single;
        }
    }
}
