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
        
        
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))] 
        private string SpeedAttribute { get; set; }
        
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))] 
        private string RangeAttribute { get; set; }
        
        [field: SerializeField, Tag] private List<string> TargetTags { get; set; } = new List<string>();
        [field: SerializeField, MinValue(0)] private float SpeedCoefficient { get; set; } = 1f;
        
        private AdvancedDropdownList<string> AttributeOptions => this.GetAttributeOptions();
        public GameObject Owner => this.Info.Damage.Instigator;
        
        public void Awake() {
            this.Info.Effects.AddRange(this.GetComponentsInChildren<IProjectileEffect>(includeInactive: true));
            this.Info.TargetTags.AddRange(this.TargetTags);
        }

        public void AddEffect(Type type, GameplayEffect effect) {
            this.Info.GameplayEffects[type] = effect;
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
            this.Info.Effects.ForEach(effect => effect.FetchAttributes(source));
            IEnumerable<IProjectileEffect> effects =
                    this.Info.Effects.Where(effect => this.Info.GameplayEffects.ContainsKey(effect.GetType()));
            foreach (IProjectileEffect effect in effects) {
                effect.TurnOn(this);
            }
            
            this.Info.IsAlive = true;
            this.Collider.includeLayers = mask;
        }

        public override void Return() {
            this.OnHitAction = delegate { };
            this.Info.Effects.ForEach(effect => effect.TurnOff(this));
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
            this.Info.Effects.ForEach(effect => effect.Execute(this, this.Collider.includeLayers, this.Info.TargetTags));
            if (!this.Info.IsAlive) {
                this.Return();
            }
        }

        public GameplayEffect GetEffect(IProjectileEffect effect) {
            return this.Info.GameplayEffects[effect.GetType()];
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
