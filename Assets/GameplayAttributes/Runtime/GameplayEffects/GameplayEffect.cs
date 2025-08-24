using System;
using System.Collections.Generic;
using GameplayAttributes.Runtime.GameplayEffects.Executions;
using GameplayAttributes.Runtime.Modifiers;
using SaintsField;
using UnityEngine;

namespace GameplayAttributes.Runtime.GameplayEffects {
    [Serializable]
    public class GameplayEffect {
        public enum Outcome {
            Success,
            Failure,
            Error,
            Cancelled
        }

        [field: SerializeReference] private EffectExecution Executor { get; set; } = new ModifierEffectExecution();
        [field: SerializeField] private bool HasLevel { get; set; }

        [field: SerializeField, Tooltip("This curve describes how a level is mapped to a modifier value coefficient")]
        [field: ShowIf(nameof(this.HasLevel))]
        private AnimationCurve LevelEffect { get; set; } = AnimationCurve.Linear(-1, -1, 1, 1);

        /// <summary>
        /// Invoke the gameplay effect to produce a set of modifiers.
        /// </summary>
        /// <param name="target">The target of the gameplay effect.</param>
        /// <param name="chance">The probability of the gameplay effect being activated on the target.</param>
        /// <param name="args">The arguments used to invoke the gameplay effect.</param>
        /// <param name="outcome">The modifiers produced by the gameplay effect.</param>
        /// <returns>The result of the gameplay effect invocation.</returns>
        public Outcome Execute(
            AttributeSet target, int chance, GameplayEffectExecutionArgs args, out IEnumerable<Modifier> outcome
        ) {
            return this.HasLevel
                    ? this.Executor.Execute(target, chance, args, out outcome, this.LevelEffect.Evaluate(args.Level))
                    : this.Executor.Execute(target, chance, args, out outcome);
        }
    }
}
