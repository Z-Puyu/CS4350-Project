using System;
using System.Collections.Generic;
using DataStructuresForUnity.Runtime.GeneralUtils;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.Projectiles {
    public sealed class ProjectileEffectController : PoolableObject {
        [field: SerializeField, Required, Expandable, DefaultExpand] 
        private ProjectileEffect Effect { get; set; }
        
        [field: SerializeField] private ParticleSystem ParticlesOnFly { get; set; }
        [field: SerializeField] private ParticleSystem ParticlesOnHit { get; set; }
        public bool EndsSilently => this.Effect.EndsSilently;
        
        private Dictionary<string, int> Attributes { get; } = new Dictionary<string, int>();
        
        public override string PoolableId => this.Effect.Id;

        public void UpdateAttribute(string key, int change) {
            this.Attributes[key] += change;
        }

        public int Get(string attribute) {
            return this.Attributes[attribute];
        }
        
        public void TurnOn(Projectile projectile) {
            foreach (AttributeEntry attribute in this.Effect.Attributes) {
                this.Attributes[attribute.Id] = projectile.GetAttribute(attribute.Id, attribute.Value);
            }
            
            this.gameObject.SetActive(true);
            if (this.ParticlesOnFly) {
                this.ParticlesOnFly.Play();
            }
        }
        
        public float TurnOff(Projectile projectile) {
            this.Attributes.Clear();
            if (this.ParticlesOnFly) {
                this.ParticlesOnFly.Stop();
            }
            
            if (this.ParticlesOnHit && this.ParticlesOnHit.isPlaying) {
                return this.ParticlesOnHit.main.loop
                        ? 1f
                        : this.ParticlesOnHit.main.duration - this.ParticlesOnHit.time;
            }
            
            return 1f;
        }

        public void Execute(Projectile projectile, LayerMask mask, IEnumerable<string> tags) {
            if (this.ParticlesOnHit) {
                this.ParticlesOnHit.Play();
            }
            
            if (this.ParticlesOnFly) {
                this.ParticlesOnFly.Stop();
            }
            
            this.Effect.Execute(projectile, mask, tags, this);
        }
    }
}
