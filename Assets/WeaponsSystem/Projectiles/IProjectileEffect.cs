using System.Collections.Generic;
using GameplayAbilities.Runtime.Attributes;
using UnityEngine;

namespace WeaponsSystem.Projectiles {
    public interface IProjectileEffect {
        public ProjectileEffectType EffectType { get; }
        
        public void TurnOn(Projectile projectile);
        
        public void TurnOff(Projectile projectile);
        
        public void Execute(Projectile projectile, LayerMask mask, IEnumerable<string> tags);
        
        public void FetchAttributes(IAttributeReader source);
    }
}
