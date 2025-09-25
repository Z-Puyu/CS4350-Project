using System;
using System.Collections.Generic;
using DataStructuresForUnity.Runtime.Trie;
using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using UnityEngine;
using WeaponsSystem.DamageHandling;

namespace WeaponsSystem.Projectiles {
    public sealed class ProjectileInfo {
        public bool IsAlive { get; set; }
        public TrieDictionary<string, char, int> Attributes { get; } = new TrieDictionary<string, char, int>();
        public Vector3 Velocity { get; set; }
        public float Range { get; set; }
        public float DistanceTravelled { get; set; }    
        public Damage Damage { get; set; }
        public List<string> TargetTags { get; } = new List<string>();
        public AttributeSet SourceWeapon { get; set; }
        public Dictionary<Type, ProjectileEffectData> Effects { get; } = new Dictionary<Type, ProjectileEffectData>();
        
        public void AddEffect(ProjectileEffectData data) {
            if (!this.Effects.TryAdd(data.Type, data)) {
                Debug.LogError($"Duplicate data for {data.Type}!");
            }
        }

        public void Reset() {
            this.IsAlive = false;
            this.Velocity = Vector3.zero;
            this.Range = 0f;
            this.DistanceTravelled = 0f;
            this.Damage = null;
            this.SourceWeapon = null; 
            this.TargetTags.Clear();
            this.Attributes.Clear();
            this.Effects.Clear();
        }
    }
}
