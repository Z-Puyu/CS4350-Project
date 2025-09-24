using System.Collections.Generic;
using UnityEngine;

namespace WeaponsSystem.Projectiles {
    public interface IProjectileEffect {
        public bool IsEnabledFor(Projectile projectile);

        public void TurnOn(Projectile projectile);
        
        public void TurnOff(Projectile projectile);
        
        public void Execute(Projectile projectile);
        
        public IEnumerable<string> GetRequiredAttributes();
    }
}
