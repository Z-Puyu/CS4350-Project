using System;
using UnityEngine;

namespace Projectiles.Runtime {
    [RequireComponent(typeof(CapsuleCollider2D))]
    public sealed class Projectile2D : Projectile {
        private void OnTriggerEnter2D(Collider2D other) {
            if (!this.IsValidTarget(other.gameObject)) {
                return;
            }

            this.Impact(other.gameObject);
        }
    }
}
