using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.GameplayEffects.Executions {
    /// <summary>
    /// A gameplay effect that can be applied to an <see cref="AttributeSet"/>.
    /// Each gameplay effect asset defines a set of parameters which are used to generate changes in attributes in run-time.
    /// </summary>
    [Serializable]
    public sealed class ModifierEffectExecution : EffectExecution {
        [field: SerializeField, Table] private List<ModifierData> Modifiers { get; set; }

        protected override IEnumerable<Modifier> Run(AttributeSet target, GameplayEffectExecutionArgs args) {
            return this.Modifiers.Select(data => data.CreateModifier(target, args));
        }

        public override int CompareTo(EffectExecution other) {
            if (object.ReferenceEquals(this, other)) {
                return 0;
            }

            if (other is not ModifierEffectExecution execution) {
                return string.CompareOrdinal(other.GetType().Name, this.GetType().Name);
            }

            if (this.Modifiers.Count > execution.Modifiers.Count) {
                return 1;
            }
            
            List<ModifierData> sorted = this.Modifiers.OrderBy(modifier => modifier).ToList();
            List<ModifierData> otherSorted = execution.Modifiers.OrderBy(modifier => modifier).ToList();
            for (int i = 0; i < this.Modifiers.Count; i += 1) {
                int comp = sorted[i].CompareTo(otherSorted[1]);
                if (comp != 0) {
                    return comp;
                }
            }

            return 0;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            foreach (ModifierData modifier in this.Modifiers.OrderBy(modifier => modifier)) {
                sb.AppendLine(modifier.ToString());
            }
            
            return sb.ToString();
        }

        public override int GetHashCode() {
            return this.ToString().GetHashCode();
        }
    }
}
