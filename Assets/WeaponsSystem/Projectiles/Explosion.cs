using System.Collections.Generic;
using System.Linq;
using Common;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using Unity.VisualScripting;
using UnityEngine;
using WeaponsSystem.DamageHandling;

namespace WeaponsSystem.Projectiles {
    [CreateAssetMenu(fileName = "Explosion Effect", menuName = "Projectiles/Effects/Explosion")]
    public sealed class Explosion : ProjectileEffect {
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))] 
        private string ExplosionRadiusAttribute { get; set; }
        
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))]
        private List<string> DamageAttributes { get; set; } = new List<string>();

        public override void Execute(
            Projectile projectile, LayerMask mask, IEnumerable<string> tags, ProjectileEffectController controller
        ) {
            int radius = controller.Get(this.ExplosionRadiusAttribute);
#if DEBUG
            OnScreenDebugger.Log($"Exploding with radius: {radius}");
#endif
            Collider2D[] colliders = Physics2D.OverlapCircleAll(projectile.transform.position, radius, mask);
            List<string> targets = tags.ToList();
            foreach (Collider2D c in colliders) {
                if (targets.Count > 0 && !targets.Any(c.CompareTag)) {
                    continue;
                }

                if (!c.TryGetComponent(out IDamageable damageable)) {
                    continue;
                }

                IReadOnlyDictionary<string, int> damageAttributes = 
                        this.DamageAttributes.ToDictionary(key => key, controller.Get);
                Damage damage = new Damage(projectile.Owner.root, projectile.Owner.combatant, damageAttributes);
                damageable.HandleDamage(damage);
            }
        }

        protected override ICollection<string> RequiredAttributes =>
                this.DamageAttributes.Append(this.ExplosionRadiusAttribute).ToArray();
    }
}
