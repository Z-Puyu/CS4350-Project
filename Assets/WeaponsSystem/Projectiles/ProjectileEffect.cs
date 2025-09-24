using UnityEngine;

namespace WeaponsSystem.Projectiles {
    [RequireComponent(typeof(Projectile))]
    public abstract class ProjectileEffect : MonoBehaviour {
        public abstract void Execute();
    }
}
