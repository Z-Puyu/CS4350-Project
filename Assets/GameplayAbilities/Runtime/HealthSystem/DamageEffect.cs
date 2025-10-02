using System;
using System.Collections.Generic;
using System.Linq;
using DataStructuresForUnity.Runtime.Utilities;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Modifiers;
using GameplayEffects.Runtime;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.HealthSystem {
    [Serializable]
    public sealed class DamageEffect : IEffect<IDataReader<string, int>, AttributeSet> {
        private readonly struct Instance : IRunnableEffect {
            private IDataReader<string, int> Source { get; }
            private AttributeSet Target { get; }
            private IEnumerable<Modifier> Modifiers { get; }
            
            public Instance(AttributeSet target, IDataReader<string, int> source, IEnumerable<Modifier> modifiers) {
                this.Target = target;
                this.Source = source;
                this.Modifiers = modifiers;
            }
            
            public void Start() {
                foreach (Modifier modifier in this.Modifiers) {
                    this.Target.AddModifier(modifier);
                }
            }
            
            public void Stop() { }

            public void Cancel() {
                foreach (Modifier modifier in this.Modifiers) {
                    this.Target.RemoveModifier(modifier);
                }
            }
        }
        
        [field: SerializeField, Table]
        private List<DamageType> DamageTypes { get; set; } = new List<DamageType>();

        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))] 
        private string TargetAttribute { get; set; }
        
        private AdvancedDropdownList<string> AttributeOptions => this.GetAttributeOptions();
        public double EffectDuration => 0;
        
        public IRunnableEffect Apply(IDataReader<string, int> source, AttributeSet projectile) {
            return new Instance(
                projectile, source, this.DamageTypes.Select(dmg => dmg.ToModifier(source, this.TargetAttribute))
            );
        }
    }
}
