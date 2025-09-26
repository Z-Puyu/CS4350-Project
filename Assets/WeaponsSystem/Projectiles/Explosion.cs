using System.Collections.Generic;
using System.Linq;
using Common;
using DataStructuresForUnity.Runtime.GeneralUtils;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using SaintsField;
using Unity.VisualScripting;
using UnityEngine;
using WeaponsSystem.DamageHandling;

namespace WeaponsSystem.Projectiles {
    [DisallowMultipleComponent]
    public sealed class Explosion : ProjectileEffect {
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))] 
        private string ExplosionRadiusAttribute { get; set; }
        
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))]
        private List<string> DamageAttributes { get; set; } = new List<string>();
        
        private int ExplosionRadius { get; set; }
        
        private AdvancedDropdownList<string> AttributeOptions => this.GetAttributeOptions();

        private Damage GetDamageData(Projectile projectile) {
            Damage damage = new Damage(projectile.Owner.root, projectile.Owner.combatant, this.Attributes);
            if (projectile.HasEffect(this.EffectType, out ProjectileEffectData data)) {
                ObjectSpawner.Pull(
                    data.ParticleAsset.PoolableId, data.ParticleAsset, this.transform.position, Quaternion.identity
                );
            }

            return damage;
        }

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
                
                damageable.HandleDamage(this.GetDamageData(projectile));
            }
        }

        public override void FetchAttributes(IAttributeReader source) {
            this.ExplosionRadius = source.GetCurrent(this.ExplosionRadiusAttribute);
            this.Attributes.Clear();
            foreach (string attribute in this.DamageAttributes) {
                this.Attributes.Add(attribute, source.GetCurrent(attribute));
            }
        }
    }
}
