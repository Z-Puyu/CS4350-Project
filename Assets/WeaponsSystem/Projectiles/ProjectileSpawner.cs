using System;
using System.Collections;
using System.Collections.Generic;
using DataStructuresForUnity.Runtime.GeneralUtils;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;
using WeaponsSystem.DamageHandling;

namespace WeaponsSystem.Projectiles {
    public sealed class ProjectileSpawner : MonoBehaviour {
        public enum Mode {
            None,
            Single,
            Spread,
            Parallel
        }


        [field: SerializeField, Required, TreeDropdown(nameof(this.AttributeOptions))]
        public string ProjectileSpreadAttribute { get; private set; }
        
        [field: SerializeField, Required, TreeDropdown(nameof(this.AttributeOptions))]
        public string ProjectilesPerShotAttribute { get; private set; }
        
        [field: SerializeField, Required, TreeDropdown(nameof(this.AttributeOptions))]
        public string ExplosionRadiusAttribute { get; private set; }
        
        [field: SerializeField, Required, TreeDropdown(nameof(this.AttributeOptions))]
        public string ParallelProjectileSpacingAttribute { get; private set; }

        private AdvancedDropdownList<string> AttributeOptions => this.GetAttributeOptions();

        private static void SpawnSingleBullet(
            Projectile prefab, IAttributeReader source, Vector3 position, ProjectileConfig config, Damage damage,
            Action<Vector3> onHit
        ) {
            if (!prefab) {
                return;
            }

            ObjectSpawner.Pull(prefab.PoolableId, prefab, position, Quaternion.identity)
                         .WithDamage(damage)
                         .Targets(config.TargetTags)
                         .OnHit(onHit)
                         .WithEffects(config.Effects)
                         .Launch(source, config.Direction, config.Mask);
        }

        private void SpawnSpreadBullet(
            Projectile prefab, IAttributeReader source, int spread, int multiplicity, ProjectileConfig config,
            Damage damage, Action<Vector3> onHit
        ) {
            float startAngle = -spread / 2.0f;
            float angleStep = spread / (multiplicity - 1.0f);
            for (int i = 0; i < multiplicity; i += 1) {
                float currentAngle = startAngle + i * angleStep;
                Vector3 currentDirection = Quaternion.Euler(0, 0, currentAngle) * config.Direction;
                ProjectileConfig newConfig = new ProjectileConfig(
                    config.Count, config.Interval, config.Mode, config.Mask, config.TargetTags, currentDirection);
                ProjectileSpawner.SpawnSingleBullet(prefab, source, this.transform.position, newConfig, damage, onHit);
            }
        }

        private void SpawnParallelBullet(
            Projectile prefab, IAttributeReader source, float spacing, int multiplicity, ProjectileConfig config,
            Damage damage, Action<Vector3> onHit
        ) {
            Vector3 orthogonal = Vector3.Cross(config.Direction, Vector3.forward).normalized;
            float interval = spacing / (multiplicity - 1.0f);
            float startOffset = -(spacing / 2.0f);
            for (int i = 0; i < multiplicity; i += 1) {
                Vector3 position = this.transform.position + (startOffset + interval * i) * orthogonal;
                ProjectileSpawner.SpawnSingleBullet(prefab, source, position, config, damage, onHit);
            }
        }

        public IEnumerator Spawn(
            Projectile prefab, IAttributeReader source, ProjectileConfig config, Damage damage, Action<Vector3> onHit
        ) {
            if (config.Mode == Mode.None) {
                yield break;
            }
            
            for (int i = 0; i < config.Count; i += 1) {
                switch (config.Mode) {
                    case Mode.Spread:
                        int spread = source.GetCurrent(this.ProjectileSpreadAttribute);
                        int multiplicity = source.GetCurrent(this.ProjectilesPerShotAttribute);
                        this.SpawnSpreadBullet(prefab, source, spread, multiplicity, config, damage, onHit);
                        break;
                    case Mode.Parallel:
                        float spacing = source.GetCurrent(this.ParallelProjectileSpacingAttribute) / 1000.0f;
                        multiplicity = source.GetCurrent(this.ProjectilesPerShotAttribute);
                        this.SpawnParallelBullet(prefab, source, spacing, multiplicity, config, damage, onHit);
                        break;
                    case Mode.Single:
                    default:
                        ProjectileSpawner.SpawnSingleBullet(
                            prefab, source, this.transform.position, config, damage, onHit
                        );
                        break;
                }

                yield return new WaitForSeconds(config.Interval / 1000.0f);
            }
        }
    }
}
