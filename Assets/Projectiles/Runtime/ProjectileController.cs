using UnityEngine;

namespace Projectiles.Runtime {
    public abstract class ProjectileController : IProjectileController {
        protected Projectile Projectile { get; set; }

        public virtual void Possess(Projectile projectile) {
            this.Projectile = projectile;
        }

        public virtual void ReleaseControl() {
            this.Projectile = null;       
        }
        
        public abstract void Update();
        public abstract void ProcessHit(Vector3 position, GameObject target);
    }
}
