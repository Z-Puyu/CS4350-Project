using System.Collections.Generic;
using UnityEngine;

namespace WeaponsSystem.Projectiles {
    public abstract class ProjectileEffect : MonoBehaviour, IProjectileEffect {
        public abstract bool IsEnabledFor(Projectile projectile);

        public virtual void TurnOn(Projectile projectile) {
            this.gameObject.SetActive(true);
        }

        public virtual void TurnOff(Projectile projectile) {
            this.gameObject.SetActive(false);
        }
        
        public abstract void Execute(Projectile projectile);
        public abstract IEnumerable<string> GetRequiredAttributes();
    }
}
