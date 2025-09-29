using System;
using UnityEngine;

namespace Projectiles.Runtime {
    [Serializable]
    public class ProjectileDurabilityController : ProjectileController {
        [field: SerializeField] public int Durability { private get; set; }
        
        public override void Possess(Projectile projectile) {
            base.Possess(projectile);
            projectile.OnHit += this.ProcessHit;
        }
        
        public override void ReleaseControl() {
            this.Projectile.OnHit -= this.ProcessHit;
            base.ReleaseControl();
        }
        
        public override void Update() { }

        public override void ProcessHit(Vector3 position, GameObject target) {
            if (!this.Projectile) {
                return;
            }
            
            this.Durability -= 1;
            if (this.Durability > 0) {
                this.Projectile.Relaunch();
            } 
        }
    }
}
