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

        protected override string GenerateSortKey() {
            List<ModifierData> data = this.Modifiers.ToList();
            data.Sort();
            return $"{this.GetType().FullName}" + 
                   $"_Modifiers:{string.Join('_', data.Select(modifier => modifier.SortKey))}";
        }

        protected override IEnumerable<Modifier> Run(AttributeSet target, GameplayEffectExecutionArgs args) {
            return this.Modifiers.Select(data => data.CreateModifier(target, args));
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            foreach (ModifierData modifier in this.Modifiers.OrderBy(modifier => modifier)) {
                sb.AppendLine(modifier.ToString());
            }
            
            return sb.ToString();
        }
    }
}
