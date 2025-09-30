using System;
using System.Collections.Generic;
using System.Linq;
using DataStructuresForUnity.Runtime.Utilities;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Modifiers;
using GameplayEffects.Runtime;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.DamageHandling {
    [Serializable]
    public sealed class DamageEffect : IEffect<IDataReader<string, int>, AttributeSet> {
        private readonly struct Instance : IRunnableEffect {
            private AttributeSet Target { get; }
            private List<Modifier> Modifiers { get; }

            public Instance(AttributeSet target, List<Modifier> modifiers) {
                this.Target = target;
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
            return new Instance(projectile, this.DamageTypes.Select(convert).Where(modifier => modifier > 0).ToList());

            Modifier convert(DamageType damage) {
                if (!source.HasValue(damage.DamageAttribute, out int magnitude) || magnitude == 0) {
                    return Modifier.Empty;
                }
                
                int defence = projectile.GetCurrent(damage.DefenceAttribute);
                magnitude = damage.IsPercentageDefence
                        ? Mathf.RoundToInt(magnitude * Math.Min(0, 100 - defence * damage.DefenceCoefficient) / 100.0f)
                        : magnitude - defence * damage.DefenceCoefficient;
                return magnitude <= 0
                        ? Modifier.Empty
                        : new Modifier(-magnitude, Modifier.Operation.Offset, this.TargetAttribute);
            }
        }
    }
}
