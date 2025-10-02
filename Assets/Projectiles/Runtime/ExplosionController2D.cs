using System;
using System.Collections.Generic;
using System.Linq;
using SaintsField;
using UnityEngine;

namespace Projectiles.Runtime {
    public sealed class ExplosionController2D : ProjectileController {
        public Func<Vector3, IEnumerable<Collider2D>> CandidateTargetGetter { private get; set; }
        [field: SerializeField] private ParticleSystem ParticleOnHit { get; set; }

        protected override bool IsIdle => this.ParticleOnHit && !this.ParticleOnHit.IsAlive();

        public override void Update() { }

        public override void Start() { }

        public override void ProcessHit(Vector3 position, GameObject target) {
            if (this.ParticleOnHit) {
                this.ParticleOnHit.Play();  
            }
            
            foreach (Collider2D c in this.CandidateTargetGetter?.Invoke(position) ?? Enumerable.Empty<Collider2D>()) {
                if (!this.Projectile.IsValidTarget(c.gameObject)) {
                    continue;
                }

                base.ProcessHit(position, c.gameObject);
            }
        }
    }
}
