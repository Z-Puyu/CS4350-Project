using UnityEngine;

namespace GameplayAbilities.Runtime.Projectiles {
    public sealed class ExplosionEffect2D : ProjectileEffect {
        [field: SerializeField] private string ExplosionRadiusAttribute { get; set; }
        [field: SerializeField] private LayerMask LayerMask { get; set; }

        protected override void HandleHit(
            Vector3 position, GameObject obj, AbilityProjectile projectile, IDataReader<string, int> sender
        ) {
            int radius = projectile.GetAttribute(this.ExplosionRadiusAttribute);
#if DEBUG
            Debug.Log($"Exploding with radius: {radius}");
#endif
            Collider2D[] colliders = Physics2D.OverlapCircleAll(projectile.transform.position, radius, this.LayerMask);
            foreach (Collider2D c in colliders) {
                if (!projectile.IsValidTarget(c)) {
                    continue;
                }

                if (!c.TryGetComponent(out IProjectileEffectHandler handler)) {
                    continue;
                }

                handler.Handle(this.Effect, sender);
            }
        }
    }
}
