using UnityEngine;

namespace GameplayAbilities.Runtime.Projectiles {
    [DisallowMultipleComponent]
    public class ProjectileHandler : MonoBehaviour, IProjectileHandler {
        [field: SerializeField, Required] private AttributeSet AttributeSet { get; set; }
        
        public void Handle(AbilityProjectile projectile) {
            projectile.Impact(this.AttributeSet);
        }
    }
}
