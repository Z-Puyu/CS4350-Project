using System;
using UnityEngine;

namespace Projectiles.Runtime {
    public abstract class ProjectileController : IProjectileController {
        protected Projectile Projectile { get; set; }
        private event Action<Vector3, GameObject> OnHit;
        
        event Action<Vector3, GameObject> IProjectileController.OnHit {
            add => this.OnHit += value;
            remove => this.OnHit -= value;
        }

        protected virtual bool IsIdle => true;
        bool IProjectileController.IsIdle => this.IsIdle;

        public virtual void Possess(Projectile projectile) {
            this.Projectile = projectile;
        }

        public virtual void ReleaseControl() {
            this.Projectile = null;       
        }
        
        public abstract void Update();

        public virtual void ProcessHit(Vector3 position, GameObject target) {
            this.OnHit?.Invoke(position, target);
        }
        
        public abstract void Start();
    }
}
