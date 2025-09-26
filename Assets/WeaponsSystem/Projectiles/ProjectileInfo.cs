using System.Collections.Generic;
using DataStructuresForUnity.Runtime.Trie;
using GameplayAbilities.Runtime.Attributes;
using UnityEngine;
using WeaponsSystem.DamageHandling;

namespace WeaponsSystem.Projectiles {
    public sealed class ProjectileInfo {
        public bool IsAlive { get; set; }
        public Vector3 Velocity { get; set; }
        public float Range { get; set; }
        public float DistanceTravelled { get; set; }    
        public Damage Damage { get; set; }
        public List<string> TargetTags { get; } = new List<string>();
        public AttributeSet SourceWeapon { get; set; }

        public void Reset() {
            this.IsAlive = false;
            this.Velocity = Vector3.zero;
            this.Range = 0f;
            this.DistanceTravelled = 0f;
            this.Damage = null;
            this.SourceWeapon = null; 
            this.TargetTags.Clear();
        }
    }
}
