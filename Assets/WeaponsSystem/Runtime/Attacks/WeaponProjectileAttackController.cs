using System.Collections.Generic;
using System.Linq;
using Projectiles.Runtime;
using UnityEngine;
using WeaponsSystem.Runtime.Weapons;

namespace WeaponsSystem.Runtime.Attacks {
    public class WeaponProjectileAttackController : WeaponAttackController {
        public ProjectileShooterMode ProjectileMode { private get; set; }

        public override void UpdatePostAttack() {
            base.UpdatePostAttack();
            this.ProjectileMode = ProjectileShooterMode.Single;
        }

        protected override AttackContext ContextOf(ref AttackAction action) {
            AttackContext context = base.ContextOf(ref action);
            return new AttackContext(
                context.Instigator, context.Owner, context.AttackableLayers, context.AttackableTags,
                context.AttackPoint, context.AttackDirection, context.WeaponStats, this.ProjectileMode
            );
        }
    }
}
