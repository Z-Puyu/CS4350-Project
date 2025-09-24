using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Modifiers;

namespace GameplayAbilities.Runtime.GameplayEffects {
    public class GameplayEffect {
        public enum Outcome {
            Success,
            Failure,
            Cancelled
        }
        
        private bool HasNeverExecuted { get; set; } = true;
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
            if (!this.Data.CanCommit(target)) {
                return Outcome.Cancelled;
            }

            return this.Data.CanMiss ? this.Data.Try(target, chance, this.Args) : Outcome.Success;
        }
        
        private IEnumerable<Modifier> Execute(AttributeSet target) {
            if (this.HasNeverExecuted) {
                this.Data.OnFirstExecution(target, this.Args);
                this.HasNeverExecuted = false;
            }

            return this.Data.Run(target, this.Args).Select(modifier => modifier * this.Args.Level);
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
        internal void Revert(AttributeSet target) {
            if (this.Data.ExecutionTime != GameplayEffectData.Periodicity.Continuous) {
                return;
            }
            
            foreach (Modifier modifier in this.Execute(target)) {
                target.AddModifier(-modifier);
            }
        }
    }
}
