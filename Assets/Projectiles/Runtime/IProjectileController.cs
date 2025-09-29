using UnityEngine;

namespace Projectiles.Runtime {
    public interface IProjectileController {
        public void Possess(Projectile projectile);
        public void ReleaseControl();
        public void Update();
        public void ProcessHit(Vector3 position, GameObject target);
    }
}
