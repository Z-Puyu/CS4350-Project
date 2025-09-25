using System;
using System.Collections.Generic;
using System.Linq;
using DataStructuresForUnity.Runtime.GeneralUtils;
using DataStructuresForUnity.Runtime.Trie;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using SaintsField;
using UnityEngine;
using Utilities;
using WeaponsSystem.DamageHandling;
using Object = UnityEngine.Object;

namespace WeaponsSystem.Projectiles {
    [DisallowMultipleComponent, RequireComponent(typeof(CapsuleCollider2D))]
    public sealed class Projectile : PoolableObject {
        private CapsuleCollider2D Collider { get; set; }
        private ProjectileInfo Info { get; } = new ProjectileInfo();
        private Action<Vector3> OnHitAction { get; set; } = delegate { };
        private List<IProjectileEffect> Effects { get; } = new List<IProjectileEffect>();
        
        
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))] 
        private string SpeedAttribute { get; set; }
        
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))] 
        private string RangeAttribute { get; set; }
        
        [field: SerializeField, Tag] private List<string> TargetTags { get; set; } = new List<string>();
        [field: SerializeField, MinValue(0)] private float SpeedCoefficient { get; set; } = 1f;
        
        private AdvancedDropdownList<string> AttributeOptions => this.GetAttributeOptions();
        public GameObject Owner => this.Info.Damage.Instigator;
        
        public void Awake() {
            this.Effects.AddRange(this.GetComponentsInChildren<IProjectileEffect>(includeInactive: true));
            this.Info.TargetTags.AddRange(this.TargetTags);
            this.Collider = this.GetComponent<CapsuleCollider2D>();
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

        public Projectile WithEffects(IEnumerable<ProjectileEffectData> effects) {
            foreach (ProjectileEffectData effect in effects) {
                this.Info.AddEffect(effect);
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

        public bool HasEffect<E>(out ProjectileEffectData data) where E : IProjectileEffect {
            return this.Info.Effects.TryGetValue(typeof(E), out data);
        }

        public void Launch(IAttributeReader source, Vector3 dir, LayerMask mask) {
            this.Info.Velocity = dir * (source.GetCurrent(this.SpeedAttribute) * this.SpeedCoefficient);
            this.Info.Range = source.GetCurrent(this.RangeAttribute);
            foreach (IProjectileEffect effect in this.Effects) {
                if (!this.Info.Effects.ContainsKey(effect.GetType())) {
                    continue;
                }

                effect.TurnOn(this);
                effect.FetchAttributes(source);
            }
            
            this.Info.IsAlive = true;
            this.Collider.includeLayers = mask;
        }

        public override void Return() {
            this.OnHitAction = delegate { };
            this.Effects.ForEach(effect => effect.TurnOff(this));
            this.Info.Reset();
            this.Collider.includeLayers = 0;
            base.Return();
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
                this.Return();
            }
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
                this.Return();
                return;
            }

            Vector3 distanceTravelledThisFrame = Time.deltaTime * this.Info.Velocity;
            this.Info.DistanceTravelled += distanceTravelledThisFrame.magnitude;
            this.transform.position += distanceTravelledThisFrame;
        }
    }
}
