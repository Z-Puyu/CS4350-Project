using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataStructuresForUnity.Runtime.GeneralUtils;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;
using WeaponsSystem.DamageHandling;

namespace WeaponsSystem.Projectiles {
    [DisallowMultipleComponent, RequireComponent(typeof(CapsuleCollider2D))]
    public sealed class Projectile : PoolableObject {
        private Transform Transform { get; set; }
        [field: SerializeField] private GameObject Visual { get; set; }
        [field: SerializeField] private string ProjectileId { get; set; }
        private CapsuleCollider2D Collider { get; set; }
        private ProjectileInfo Info { get; } = new ProjectileInfo();
        private Action<Vector3> OnHitAction { get; set; } = delegate { };
        private List<ProjectileEffectController> Effects { get; } = new List<ProjectileEffectController>();
        
        
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))] 
        private string SpeedAttribute { get; set; }
        
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))] 
        private string RangeAttribute { get; set; }
        
        [field: SerializeField, Tag] private List<string> TargetTags { get; set; } = new List<string>();
        [field: SerializeField, MinValue(0)] private float SpeedCoefficient { get; set; } = 1f;
        
        private AdvancedDropdownList<string> AttributeOptions => this.GetAttributeOptions();
        public (GameObject root, Combatant combatant) Owner => this.Info.Damage.Instigator;
        public override string PoolableId => this.ProjectileId;
        
        public void Awake() {
            this.Transform = this.transform;
            this.Info.TargetTags.AddRange(this.TargetTags);
            this.Collider = this.GetComponent<CapsuleCollider2D>();
        }

        public int GetAttribute(string id, int @base) {
            return this.Info.SourceWeapon.Query(id, @base);
        }
        
        public Projectile Targets(IEnumerable<string> tags) {
            foreach (string t in tags) {
                if (this.Info.TargetTags.Contains(t)) {
                    continue;
                }
                
                this.Info.TargetTags.Add(t);
            }

            return this;
        }

        public Projectile Targets(params string[] tags) {
            foreach (string t in tags) {
                if (this.Info.TargetTags.Contains(t)) {
                    continue;
                }
                
                this.Info.TargetTags.Add(t);
            }

            return this;
        }

        public Projectile WithEffects(IEnumerable<ProjectileEffect> effects) {
            foreach (ProjectileEffect effect in effects) {
                ProjectileEffectController controller =
                        ObjectSpawner.Pull<ProjectileEffectController>(effect.Id, this.transform);
                this.Effects.Add(controller);
            }

            return this;
        }

        public Projectile WithDamage(Damage damage) {
            this.Info.Damage = damage;
            return this;
        }

        public Projectile OnHit(Action<Vector3> action) {
            this.OnHitAction += action;
            return this;
        }

        public void Launch(IAttributeReader source, Vector3 dir, LayerMask mask) {
            this.Info.Velocity = dir * (source.GetCurrent(this.SpeedAttribute) * this.SpeedCoefficient);
            this.Info.Range = source.GetCurrent(this.RangeAttribute);
            this.Info.SourceWeapon = source;
            foreach (ProjectileEffectController effect in this.Effects) {
                effect.TurnOn(this);
            }
            
            this.Info.IsAlive = true;
            this.Collider.includeLayers = mask;
            if (this.Visual) {
                this.Visual.SetActive(true);
            }
        }

        public void Relaunch(float rotation, float speedCoefficient = 1f) {
            this.Info.Velocity = Quaternion.AngleAxis(rotation, Vector3.forward) * 
                                 this.Info.Velocity * speedCoefficient;
            this.Info.IsAlive = true;     
        }

        public void Relaunch(Vector3 dir, float speedCoefficient = 1f) {
            this.Info.Velocity = dir.normalized * this.Info.Velocity.magnitude * speedCoefficient;
            this.Info.IsAlive = true;
        }

        public void Relaunch() {
            this.Info.IsAlive = true;
        }
        
        private void Hit(IDamageable target) {
            this.OnHitAction(this.transform.position);
            target.HandleDamage(this.Info.Damage);
            this.Info.IsAlive = false;
            this.Effects.ForEach(effect => effect.Execute(this, this.Collider.includeLayers, this.Info.TargetTags));
            if (!this.Info.IsAlive) {
                this.Destroy();
            }
        }

        private void Destroy() {
            if (this.Visual) {
                this.Visual.SetActive(false);
            }

            this.OnHitAction = delegate { };
            float waitingTime = 0;
            foreach (ProjectileEffectController effect in this.Effects) {
                if (!effect.EndsSilently) {
                    effect.Execute(this, this.Collider.includeLayers, this.Info.TargetTags);
                }
                
                waitingTime = Mathf.Max(waitingTime, effect.TurnOff(this));
            }
            
            this.Info.Reset();
            this.Collider.includeLayers = 0;
            this.StartCoroutine(this.WaitToDestroy(2 * waitingTime));
        }

        private IEnumerator WaitToDestroy(float seconds) {
            yield return new WaitForSeconds(seconds);
            this.Return();
        }

        public override void Return() {
            this.Effects.ForEach(effect => effect.Return());
            base.Return();
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (this.Info.TargetTags.Count > 0 && !this.Info.TargetTags.Any(other.gameObject.CompareTag)) {
                return;
            }

            if (other.TryGetComponent(out IDamageable damageable)) {
                this.Hit(damageable);
            }
        }

        private void Update() {
            if (!this.Info.IsAlive) {
                return;
            }
            
            if (this.Info.DistanceTravelled >= this.Info.Range) {
                this.Destroy();
                return;
            }

            Vector3 distanceTravelledThisFrame = Time.deltaTime * this.Info.Velocity;
            this.Info.DistanceTravelled += distanceTravelledThisFrame.magnitude;
            this.Transform.position += distanceTravelledThisFrame;
            this.Transform.right = this.Info.Velocity;
        }
    }
}
