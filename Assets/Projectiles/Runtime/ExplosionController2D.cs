using System;
using System.Collections.Generic;
using System.Linq;
using SaintsField;
using UnityEngine;

namespace Projectiles.Runtime {
    public sealed class ExplosionController2D : ProjectileController {
        public Func<Vector3, IEnumerable<Collider2D>> CandidateTargetGetter { private get; set; }
        [field: SerializeField] private ParticleSystem ParticleOnFly { get; set; }
        [field: SerializeField] private ParticleSystem ParticleOnHit { get; set; }
        
        public override void Update() { }

        public override void Start() {
            this.ParticleOnFly.Play();
        }

        public override void ProcessHit(Vector3 position, GameObject target) {
            this.ParticleOnFly.Stop();
            this.ParticleOnHit.Play();
            foreach (Collider2D c in this.CandidateTargetGetter?.Invoke(position) ?? Enumerable.Empty<Collider2D>()) {
                if (!this.Projectile.IsValidTarget(c.gameObject)) {
                    continue;
                }

                base.ProcessHit(position, c.gameObject);
            }
        }
    }
}
