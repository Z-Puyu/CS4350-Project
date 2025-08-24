using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAttributes.Runtime.Modifiers;
using Random = UnityEngine.Random;

namespace GameplayAttributes.Runtime.GameplayEffects.Executions {
    /// <summary>
    /// A gameplay effect that can be applied to an <see cref="AttributeSet"/>.
    /// Each gameplay effect asset defines a set of parameters which are used to generate changes in attributes in run-time.
    /// </summary>
    [Serializable]
    public abstract class EffectExecution {
        private bool HasNeverExecuted { get; set; } = true;

        /// <summary>
        /// Additional logic to execute on the first execution of the gameplay effect.
        /// </summary>
        /// <param name="args">The execution arguments.</param>
        protected virtual void OnFirstExecution(GameplayEffectExecutionArgs args) { }

        /// <summary>
        /// Run the main execution logic here.
        /// </summary>
        /// <param name="target">The target of the gameplay effect.</param>
        /// <param name="args">The additional arguments used to execute the gameplay effect.</param>
        /// <returns></returns>
        protected abstract IEnumerable<Modifier> Run(AttributeSet target, GameplayEffectExecutionArgs args);

        /// <summary>
        /// Invoke the gameplay effect to produce a set of modifiers.
        /// </summary>
        /// <param name="target">The target of the gameplay effect.</param>
        /// <param name="chance">The chance of the gameplay effect being activated on the target.</param>
        /// <param name="args">The arguments used to invoke the gameplay effect.</param>
        /// <param name="outcome">The modifiers produced by the gameplay effect.</param>
        /// <param name="level">The level of the gameplay effect.</param>
        /// <returns>The result of the gameplay effect invocation.</returns>
        public GameplayEffect.Outcome Execute(
            AttributeSet target, int chance, GameplayEffectExecutionArgs args, out IEnumerable<Modifier> outcome, float level = 1
        ) {
            bool isSuccess = chance switch {
                >= 100 => true,
                <= 0 => false,
                var _ => Random.Range(0, 100) < chance
            };

            if (!isSuccess) {
                outcome = Enumerable.Empty<Modifier>();
                return GameplayEffect.Outcome.Failure;
            }
            
            if (this.HasNeverExecuted) {
                this.OnFirstExecution(args);
                this.HasNeverExecuted = false;
            }
            
            outcome = this.Run(target, args).Select(modifier => modifier * level);
            return GameplayEffect.Outcome.Success;
        }
    }
}
