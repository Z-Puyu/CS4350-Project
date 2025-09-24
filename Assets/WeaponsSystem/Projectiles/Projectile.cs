using System;
using System.Collections.Generic;
using System.Linq;
using DataStructuresForUnity.Runtime.Trie;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;
using Utilities;
using WeaponsSystem.DamageHandling;
using Object = UnityEngine.Object;

namespace WeaponsSystem.Projectiles {
    [DisallowMultipleComponent]
    public sealed class Projectile : MonoBehaviour {
        public enum Motion {
            Pierce,
            Deflect,
            Reflect,
            Retarget,
            None
        }

        [Flags]
        public enum OnHitReaction {
            None = 0,
            Explode = 1
        }

        private bool IsAlive { get; set; }
        private List<IProjectileEffect> Effects { get; } = new();
        private TrieDictionary<string, char, int> Attributes { get; } = new TrieDictionary<string, char, int>();
        
        public Motion MotionType { get; private set; } = Motion.None;
        public OnHitReaction SpecialEffects { get; private set; } = OnHitReaction.None;
        private Vector3 Velocity { get; set; }
        private float range;
        private float distanceTravelled;
        private ObjectPool<Projectile> pool;
        private Damage Damage { get; set; }
        
        [field: SerializeField, AdvancedDropdown(nameof(this.AttributeOptions))] 
        private string SpeedAttribute { get; set; }
        
        [field: SerializeField, AdvancedDropdown(nameof(this.AttributeOptions))] 
        private string RangeAttribute { get; set; }
        
        [field: SerializeField, Tag] private List<string> TargetTags { get; set; } = new List<string>();

        [field: SerializeField, MinValue(0)] private float SpeedCoefficient { get; set; } = 1f;
        
        private AdvancedDropdownList<string> AttributeOptions => this.GetAttributeOptions();
        
        public void Awake() {
            this.Effects.AddRange(this.GetComponentsInChildren<IProjectileEffect>(includeInactive: true));
        }

        private void Start() {
            foreach (IProjectileEffect effect in this.Effects.Where(effect => effect.IsEnabledFor(this))) {
                effect.TurnOn(this);
            }
            
            this.IsAlive = true;
        }

        private IEnumerable<string> GetRequiredAttributes() {
            HashSet<string> required = new HashSet<string> { this.SpeedAttribute, this.RangeAttribute };
            foreach (IProjectileEffect effect in this.GetComponentsInChildren<IProjectileEffect>()) {
                foreach (string attribute in effect.GetRequiredAttributes()) {
                    required.Add(attribute);
                }
            }
            
            return required;
        }

        public Projectile WithAttributes(IAttributeReader attributes) {
            foreach (string id in this.GetRequiredAttributes()) {
                this.Attributes.Add(id, attributes.GetCurrent(id));
            }

            return this;
        }

        public Projectile Targets(params string[] tags) {
            foreach (string t in tags) {
                if (this.TargetTags.Contains(t)) {
                    continue;
                }
                
                this.TargetTags.Add(t);
            }

            return this;
        }

        public Projectile WithDamage(Damage damage) {
            this.Damage = damage;
            return this;
        }

        public void Launch(Vector3 dir, ObjectPool<Projectile> source) {
            this.Velocity = dir * (this.Attributes[this.SpeedAttribute] * this.SpeedCoefficient);
            this.range = this.Attributes[this.RangeAttribute];
            this.pool = source;
        }

        public int GetAttribute(string key) {
            return this.Attributes.TryGetValue(key, out int value) ? value : 0;
        }

        private void Destroy() {
            this.range = 0;
            this.distanceTravelled = 0;
            this.Velocity = Vector3.zero;
            this.Damage = null;
            this.Effects.ForEach(effect => effect.TurnOff(this));
            this.SpecialEffects = OnHitReaction.None;
            this.MotionType = Motion.None;
            this.pool.ReturnInstance(this);
        }

        public void MarkForDestruction() {
            this.IsAlive = false;
        }
        
        private void OnTriggerEnter2D(Collider2D other) {
            if (this.TargetTags.Count > 0 && !this.TargetTags.Any(other.gameObject.CompareTag)) {
                return;
            }

            if (!other.TryGetComponent(out IDamageable damageable)) {
                return;
            }

            damageable.HandleDamage(this.Damage);
            if (this.Effects.Count == 0) {
                this.MarkForDestruction();
            } else {
                this.Effects.ForEach(effect => effect.Execute(this));
            }
        }

        private void Update() {
            if (!this.IsAlive || this.distanceTravelled >= this.range) {
                this.Destroy();
                return;
            }

            Vector3 distanceTravelledThisFrame = Time.deltaTime * this.Velocity;
            this.distanceTravelled += distanceTravelledThisFrame.magnitude;
            this.transform.position += distanceTravelledThisFrame;
        }
    }
}
