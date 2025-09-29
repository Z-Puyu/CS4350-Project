using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.Modifiers;
using GameplayEffects.Runtime;
using SaintsField;
using UnityEngine;
using WeaponsSystem.Projectiles;
using ProjectileEffect = WeaponsSystem.Projectiles.ProjectileEffect;

namespace WeaponsSystem {
    [Serializable]
    public sealed class AttributeBasedAttack : IEffect<WeaponStats> {
        private readonly struct Instance : IRunnableEffect {
            private WeaponStats Target { get; }
            private List<Modifier> Modifiers { get; }

            public Instance(WeaponStats target, List<Modifier> modifiers) {
                this.Target = target;
                this.Modifiers = modifiers;
            }
            
            public void Start() {
                this.Modifiers.ForEach(this.Target.AddModifier);
            }
            
            public void Stop() { }
            
            public void Cancel() {
                this.Modifiers.ForEach(this.Target.RemoveModifier);
            }
        }
        
        [field: SerializeField] public ProjectileSpawner.Mode ProjectileMode { get; private set; }
        [field: SerializeField] public List<ProjectileEffect> ProjectileEffects { get; private set; }
        
        [field: SerializeField, Table]
        private List<ModifierData> Modifiers { get; set; } = new List<ModifierData>();

        public double EffectDuration => 0;
        
        public IRunnableEffect Apply(WeaponStats target) {
            return new Instance(target, this.Modifiers.Select(m => m.CreateModifier(target)).ToList());
        }
    }
}
