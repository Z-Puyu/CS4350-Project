using System.Collections.Generic;
using Projectiles.Runtime;
using UnityEngine;

namespace Weapons.Runtime {
    public readonly struct AttackContext {
        public Weapon Owner { get; }
        public LayerMask AttackableLayers { get; }
        public List<string> AttackableTags { get; }
        public Vector3 AttackPoint { get; }
        public WeaponStats WeaponStats { get; }
        public Vector3 Direction { get; }
        public ProjectileShooterMode ProjectileMode { get; }

        public AttackContext(
            Weapon owner, LayerMask layers, List<string> tags, Vector3 position, Vector3 direction,
            WeaponStats data, ProjectileShooterMode projectileMode = ProjectileShooterMode.None
        ) {
            this.Owner = owner;
            this.AttackableLayers = layers;
            this.AttackableTags = tags;
            this.AttackPoint = position;
            this.WeaponStats = data;
            this.Direction = direction.normalized;
            this.ProjectileMode = projectileMode;
        }
    }
}
