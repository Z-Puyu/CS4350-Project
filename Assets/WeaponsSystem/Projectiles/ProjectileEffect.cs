using System.Collections.Generic;
using GameplayAbilities.Runtime.Attributes;
using UnityEngine;

namespace WeaponsSystem.Projectiles {
    public abstract class ProjectileEffect : MonoBehaviour, IProjectileEffect {
        [field: SerializeField] private ProjectileEffectType Type { get; set; }
        protected IDictionary<string, int> Attributes { get; } = new Dictionary<string, int>();

        public ProjectileEffectType EffectType => this.Type;

        public virtual void TurnOn(Projectile projectile) {
            this.gameObject.SetActive(true);
        }

        public virtual void TurnOff(Projectile projectile) {
            this.gameObject.SetActive(false);
        }
        
        public abstract void Execute(Projectile projectile, LayerMask mask, IEnumerable<string> tags);
        public abstract void FetchAttributes(IAttributeReader source);
    }
}
