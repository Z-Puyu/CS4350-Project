using System;
using UnityEngine;

namespace Projectiles.Runtime {
    public interface IProjectileController {
        public event Action<Vector3, GameObject> OnHit;
        bool IsIdle { get; }

        public void Possess(Projectile projectile);
        public void ReleaseControl();
        public void Update();
        public void ProcessHit(Vector3 position, GameObject target);
        public void Start();
    }
}
