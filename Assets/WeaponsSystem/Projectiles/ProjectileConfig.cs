using System.Collections.Generic;
using UnityEngine;
using WeaponsSystem.DamageHandling;

namespace WeaponsSystem.Projectiles {
    public readonly struct ProjectileConfig {
        public int Count { get; }
        public int Interval { get; }
        public ProjectileSpawner.Mode Mode { get; }
        public LayerMask Mask { get; }
        public ICollection<string> TargetTags { get; }
        public Vector3 Direction { get; }

        public ProjectileConfig(
            int count, int interval, ProjectileSpawner.Mode mode, LayerMask mask, ICollection<string> targetTags,
            Vector3 direction
        ) {
            this.Count = count;
            this.Interval = interval;
            this.Mode = mode;
            this.Mask = mask;
            this.TargetTags = targetTags;
            this.Direction = direction;
        }
    }
}
