using System.Collections.Generic;
using GameplayAbilities.Runtime.Modifiers;

namespace GameplayAbilities.Runtime.GameplayEffects {
    public class GameplayEffect {
        public enum Outcome {
            Success,
            Failure,
            Cancelled
        }
        
        internal GameplayEffectData Data { get; }
        private GameplayEffectExecutionArgs Args { get; }
        
        internal GameplayEffect(GameplayEffectData data, GameplayEffectExecutionArgs args) {
            this.Data = data;
            this.Args = args;
        }

        /// <summary>
        /// Invoke the gameplay effect to produce a set of modifiers.
        /// </summary>
        /// <param name="target">The target of the gameplay effect.</param>
        /// <param name="chance">The probability of the gameplay effect being activated on the target.</param>
        /// <returns>The result of the gameplay effect invocation.</returns>
        internal Outcome Commit(AttributeSet target, int chance) {
            return !this.Data.CanCommit(target) ? Outcome.Cancelled : this.Data.Executor.Try(target, chance, this.Args);
        }
        
        private IEnumerable<Modifier> Execute(AttributeSet target) {
            return this.Data.HasLevel
                    ? this.Data.Executor.Execute(target, this.Args, this.Data.LevelEffect.Evaluate(this.Args.Level))
                    : this.Data.Executor.Execute(target, this.Args);
        }

        /// <summary>
        /// Apply the effect to the target.
        /// </summary>
        /// <param name="target">The target game object.</param>
        internal void Apply(AttributeSet target) {
            foreach (Modifier modifier in this.Execute(target)) {
                target.AddModifier(modifier);
            }
        }

        /// <summary>
        /// Terminates the gameplay effect on the target. This will revert things like temporary buffs.
        /// </summary>
        /// <param name="target">The target on which this gameplay effect has been active.</param>
        internal void EndOn(AttributeSet target) {
            if (this.Data.ExecutionTime != GameplayEffectData.Periodicity.Continuous) {
                return;
            }
            
            foreach (Modifier modifier in this.Execute(target)) {
                target.AddModifier(-modifier);
            }
        }
    }
}
