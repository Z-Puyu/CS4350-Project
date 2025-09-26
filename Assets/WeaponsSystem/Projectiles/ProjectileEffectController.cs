using System;
using System.Collections.Generic;
using DataStructuresForUnity.Runtime.GeneralUtils;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.Projectiles {
    public sealed class ProjectileEffectController : PoolableObject {
        [field: SerializeField, Required] private ProjectileEffect Effect { get; set; }
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
        }
        
        public void TurnOff(Projectile projectile) {
            this.Attributes.Clear();
        }
        
        public void Execute(Projectile projectile, LayerMask mask, IEnumerable<string> tags) {
            this.Effect.Execute(projectile, mask, tags, this);
        }
    }
}
