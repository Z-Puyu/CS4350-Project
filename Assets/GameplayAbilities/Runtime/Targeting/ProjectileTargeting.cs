using DataStructuresForUnity.Runtime.ObjectPooling;
using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.Attributes;
using Projectiles.Runtime;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.Targeting {
    public class ProjectileTargeting : TargetingStrategy {
        [field: SerializeField] private Projectile ProjectilePrefab { get; set; }
        [field: SerializeField, MinValue(1)] private float Speed { get; set; }
        [field: SerializeField, MinValue(1)] private float Range { get; set; }

        protected void HandleProjectileHit(Vector3 hitPoint, GameObject hitObject) {
#if DEBUG
            Debug.Log($"Target {hitObject.name} hit at {hitPoint}");
#endif
            if (hitObject.TryGetComponent(out IAbilityTarget target)) {
                target.Receive(this.Ability, this.Targeter.AttributeSet);
            }
        }

        protected virtual Projectile ConfigureProjectileControllers(Projectile projectile) {
            return projectile;
        }

        protected override void ConfirmTarget() {
            Vector3 origin = this.Targeter.ProjectileOrigin;
            Vector3 centre = this.Targeter.transform.position;
            this.ConfigureProjectileControllers(ObjectPools<Projectile>.Get(this.ProjectilePrefab))
                .Targeting(this.Targeter.AbilityTargets)
                .WhenHit(this.HandleProjectileHit)
                .Launch(origin, origin - centre, this.Speed, this.Range);
        }
    }
}
