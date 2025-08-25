using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
