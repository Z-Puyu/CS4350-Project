using System.Collections.Generic;
using GameplayAbilities.Runtime.Attributes;
using Projectiles.Runtime;
using UnityEngine;
using WeaponsSystem.Runtime.Weapons;

namespace WeaponsSystem.Runtime.Attacks {
    public readonly struct AttackContext {
        public GameObject Instigator { get; }
        public Weapon Owner { get; }
        public Vector3 AttackPoint { get; }
        public Vector3 AttackDirection { get; }
        public LayerMask AttackableLayers { get; }
        public List<string> AttackableTags { get; }
        public AttributeSet WeaponStats { get; }
        public ProjectileShooterMode ProjectileMode { get; }

        public AttackContext(
            GameObject instigator, Weapon owner, LayerMask attackableLayers, List<string> attackableTags,
            Vector3 attackPoint, Vector3 attackDirection, AttributeSet weaponStats,
            ProjectileShooterMode projectileMode = ProjectileShooterMode.Single
        ) {
            this.Instigator = instigator;
            this.Owner = owner;
            this.AttackPoint = attackPoint;
            this.AttackDirection = attackDirection;
            this.AttackableLayers = attackableLayers;
            this.AttackableTags = attackableTags;
            this.WeaponStats = weaponStats;
            this.ProjectileMode = projectileMode;
        }
    }
}
