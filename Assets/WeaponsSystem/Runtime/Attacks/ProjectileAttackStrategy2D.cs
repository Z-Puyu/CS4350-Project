using System;
using System.Collections;
using DataStructuresForUnity.Runtime.ObjectPooling;
using Projectiles.Runtime;
using SaintsField;
using UnityEngine;
using WeaponsSystem.Runtime.Weapons;
using Projectile = Projectiles.Runtime.Projectile;

namespace WeaponsSystem.Runtime.Attacks {
    public class ProjectileAttackStrategy2D : AttackStrategy {
        [field: SerializeField] private Projectile ProjectilePrefab { get; set; }
        
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))] 
        private string RangeAttribute { get; set; }

        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))]
        private string SpeedAttribute { get; set; }
        
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))]
        private string ProjectileSpreadAttribute { get; set; }
        
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))]
        private string ProjectilesPerShotAttribute { get; set; }
        
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))]
        private string ExplosionRadiusAttribute { get; set; }
        
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))]
        private string ParallelProjectileSpacingAttribute { get; set; }
        
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))]
        private string ShotsPerAttackAttribute { get; set; }
        
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))]
        private string IntervalBetweenShotsAttribute { get; set; }
        
        private void SpawnSpreadBullet(AttackContext context, int speed, int range) {
            int multiplicity = context.WeaponStats.GetCurrent(this.ProjectilesPerShotAttribute);
            if (multiplicity == 1) {
                this.SpawnSingleBullet(context).Launch(context.AttackPoint, context.AttackDirection, speed, range);
                return;
            }
            
            int spread = context.WeaponStats.GetCurrent(this.ProjectileSpreadAttribute);
            float startAngle = -spread / 2.0f;
            float angleStep = spread / (multiplicity - 1.0f);
            for (int i = 0; i < multiplicity; i += 1) {
                float currentAngle = startAngle + i * angleStep;
                Vector3 currentDirection = Quaternion.Euler(0, 0, currentAngle) * context.AttackDirection;
                this.SpawnSingleBullet(context).Launch(currentDirection, speed, range);
            }
        }

        private void SpawnParallelBullet(AttackContext context, int speed, int range) {
            int multiplicity = context.WeaponStats.GetCurrent(this.ProjectilesPerShotAttribute);
            if (multiplicity == 1) {
                this.SpawnSingleBullet(context).Launch(context.AttackPoint, context.AttackDirection, speed, range);
                return;
            }
            
            Vector3 orthogonal = Vector3.Cross(context.AttackDirection, Vector3.forward).normalized;
            float spacing = context.WeaponStats.GetCurrent(this.ParallelProjectileSpacingAttribute) / 1000.0f;
            float interval = spacing / (multiplicity - 1.0f);
            float startOffset = -(spacing / 2.0f);
            for (int i = 0; i < multiplicity; i += 1) {
                Vector3 position = context.AttackPoint + (startOffset + interval * i) * orthogonal;
                this.SpawnSingleBullet(context).Launch(position, context.AttackDirection, speed, range);
            }
        }
        
        private Projectile SpawnSingleBullet(AttackContext context) {
            if (!this.ProjectilePrefab) {
#if DEBUG
                Debug.LogError("Projectile prefab is not set!");
#endif
                return null;
            }
            
            return ObjectPools<Projectile>.Get(this.ProjectilePrefab, context.AttackPoint)
                                          .Targeting(context.AttackableTags)
                                          .WhenHit(handleProjectileHit);
            
            void handleProjectileHit(Vector3 hitPoint, GameObject hitObject) {
                if (hitObject.TryGetComponent(out IDamageable damageable) &&
                    this.AllowsDamageOn(hitObject, context.Instigator)) {
                    damageable.HandleDamage(this.DealDamage(context));
                }
            }
        }

        private IEnumerator Shoot(AttackContext context, int count, float interval) {
            int speed = context.WeaponStats.GetCurrent(this.SpeedAttribute);
            int range = context.WeaponStats.GetCurrent(this.RangeAttribute);
            for (int i = 0; i < count; i += 1) {
                switch (context.ProjectileMode) {
                    case ProjectileShooterMode.Spread:
                        this.SpawnSpreadBullet(context, speed, range);
                        break;
                    case ProjectileShooterMode.Parallel:
                        this.SpawnParallelBullet(context, speed, range);
                        break;
                    case ProjectileShooterMode.Single:
                    default:
                        this.SpawnSingleBullet(context).Launch(context.AttackDirection, speed, range);
                        break;
                }

                if (i < count - 1) {
                    yield return new WaitForSeconds(interval);
                }
            }
        }

        public override float Execute(ref AttackContext context) {
            int count = Math.Max(1, context.WeaponStats.GetCurrent(this.ShotsPerAttackAttribute));
            float interval = context.WeaponStats.GetCurrent(this.IntervalBetweenShotsAttribute) / 1000.0f;
            context.Owner.StartCoroutine(this.Shoot(context, count, interval));
            return interval * (count - 1);
        }
    }
}
