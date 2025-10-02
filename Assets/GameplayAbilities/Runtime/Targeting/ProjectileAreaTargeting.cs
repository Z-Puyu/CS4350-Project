using System.Collections.Generic;
using System.Linq;
using Projectiles.Runtime;
using SaintsField.Playa;
using UnityEngine;

namespace GameplayAbilities.Runtime.Targeting {
    /// <summary>
    /// Launch a projectile. On hit, all targets within an area receives the ability effects.
    /// </summary>
    public class ProjectileAreaTargeting : ProjectileTargeting {
        private enum Shape { Circular, Rectangular }

        [field: SerializeField] private LayerMask AffectedLayers { get; set; }
        [field: SerializeField] private Shape AreaShape { get; set; }
        
        [field: SerializeField, ShowIf(nameof(this.AreaShape), Shape.Circular)] 
        private int Radius { get; set; }
        
        [field: SerializeField, ShowIf(nameof(this.AreaShape), Shape.Rectangular)]
        private Vector2Int Dimensions { get; set; }

        private IEnumerable<Collider2D> SearchTargets(Vector3 centre) {
            return this.AreaShape switch {
                Shape.Circular => Physics2D.OverlapCircleAll(centre, this.Radius, this.AffectedLayers),
                Shape.Rectangular => Physics2D.OverlapBoxAll(centre, this.Dimensions, this.AffectedLayers),
                var _ => Enumerable.Empty<Collider2D>()
            };
        }

        protected override Projectile ConfigureProjectileControllers(Projectile projectile) {
            IProjectileController controller = projectile.GetController<ExplosionController2D>();
            ((ExplosionController2D)controller).CandidateTargetGetter = this.SearchTargets;
            controller.OnHit += this.HandleProjectileHit;
            return projectile;
        }
    }
}
