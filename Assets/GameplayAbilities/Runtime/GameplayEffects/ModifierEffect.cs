using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.GameplayEffects {
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

        public IRunnableEffect Apply(IDataReader<string, int> source, AttributeSet target) {
            return new Instance(target, this.Modifiers.Select(modifier => modifier.CreateModifier(target, source)));
        }
    }
}
