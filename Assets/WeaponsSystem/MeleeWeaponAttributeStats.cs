using System.Collections.Generic;
using System.Linq;
using SaintsField;
using UnityEngine;
using WeaponsSystem.Attacks;
using WeaponsSystem.Projectiles;

namespace WeaponsSystem {
    public sealed class MeleeWeaponAttributeStats : WeaponAttributeStats {
        [field: SerializeField, Required, PropRange(0, 180)] 
        public float SweepHalfAngle { get; private set; }
        
        [field: SerializeField, Required, TreeDropdown(nameof(this.AttributeOptions))] 
        public string MeleeRangeAttribute { get; private set; }

        protected override void UpdateProjectileMode(int index) {
            if (this.ProjectileEffects.Count == 0) {
                this.ProjectileMode = ProjectileSpawner.Mode.None;
            }
            
            List<AttributeBasedAttack> modifiers = this.AttackModifiers[index];
            this.ProjectileMode = modifiers.Count == 0 || modifiers.Last().ProjectileMode == ProjectileSpawner.Mode.None
                    ? ProjectileSpawner.Mode.Spread
                    : modifiers.Last().ProjectileMode;
        }
        
        protected override void RevertProjectileMode(int index) {
            this.ProjectileMode = ProjectileSpawner.Mode.None;
        }
    }
}
