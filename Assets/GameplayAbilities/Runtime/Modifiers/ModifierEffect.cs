using System;
using System.Collections.Generic;
using System.Linq;
using DataStructuresForUnity.Runtime.Utilities;
using GameplayAbilities.Runtime.Attributes;
using GameplayEffects.Runtime;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.Modifiers {
    [Serializable]
    public sealed class ModifierEffect : IEffect<IDataReader<string, int>, AttributeSet> {
        private readonly struct Instance : IRunnableEffect {
            private AttributeSet Target { get; }
            private IEnumerable<Modifier> Modifiers { get; }

            public Instance(AttributeSet target, IEnumerable<Modifier> modifiers) {
                this.Target = target;
                this.Modifiers = modifiers;
            }

            public void Start() {
                foreach (Modifier modifier in this.Modifiers) {
                    this.Target.AddModifier(modifier);
                }
            }

            void IRunnableEffect.Stop() { }
            
            public void Cancel() {
                foreach (Modifier modifier in this.Modifiers) {
                    this.Target.AddModifier(-modifier);
                }
            }
        }
        
        [field: SerializeField, Table]
        private List<ModifierData> Modifiers { get; set; } = new List<ModifierData>();

        public double EffectDuration => 0;

        public IRunnableEffect Apply(IDataReader<string, int> source, AttributeSet projectile) {
            return new Instance(projectile, this.Modifiers.Select(modifier => modifier.CreateModifier(projectile, source)));
        }
    }
}
