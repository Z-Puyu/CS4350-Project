using System.Collections.Generic;
using System.Linq;
using Common;
using DataStructuresForUnity.Runtime.GeneralUtils;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using SaintsField;
using UnityEngine;
using WeaponsSystem.DamageHandling;

namespace WeaponsSystem.Projectiles {
    [DisallowMultipleComponent]
    public sealed class Explosion : ProjectileEffect {
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))] 
        private string ExplosionRadiusAttribute { get; set; }
        
        private int ExplosionRadius { get; set; }
        
        private AdvancedDropdownList<string> AttributeOptions => this.GetAttributeOptions();

        public override void Execute(Projectile projectile, LayerMask mask, IEnumerable<string> tags) {
            OnScreenDebugger.Log($"Exploding with radius: {this.ExplosionRadius}");
            Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, this.ExplosionRadius, mask);
            List<string> targets = tags.ToList();
            foreach (Collider2D c in colliders) {
                if (targets.Count > 0 && !targets.Any(c.CompareTag)) {
                    continue;
                }
                
                if (!c.TryGetComponent(out IDamageable damageable)) {
                    continue;
                }

                Damage damage = new Damage(projectile.Owner);
                if (projectile.HasEffect<Explosion>(out ProjectileEffectData data)) {
                    ObjectSpawner.Pull(
                        data.ParticleAsset.PoolableId, data.ParticleAsset, this.transform.position, Quaternion.identity
                    );
                    
                    damage = damage.WithEffectsOnSelf(data.EffectsOnInstigator)
                                   .WithEffectsOnTarget(data.EffectsOnTarget);
                }
                
                damageable.HandleDamage(damage);
            }
        }

        public override void FetchAttributes(IAttributeReader source) {
            this.ExplosionRadius = source.GetCurrent(this.ExplosionRadiusAttribute);
        }
    }
}
