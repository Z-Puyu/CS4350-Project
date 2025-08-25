using System.Collections.Generic;
using GameplayAbilities.Runtime.Attributes;

namespace GameplayAbilities.Runtime.GameplayEffects {
    public class GameplayEffectExecutionArgs {
        public IAttributeReader Instigator { get; }
        public float Level { get; }
        public Dictionary<string, int> CallerSuppliedModifierValues { get; }

        private GameplayEffectExecutionArgs(
            IAttributeReader instigator, float level, Dictionary<string, int> callerSuppliedModifierValues
        ) {
            this.Instigator = instigator;
            this.Level = level;
            this.CallerSuppliedModifierValues = callerSuppliedModifierValues;
        }

        public sealed class Builder {
            private IAttributeReader Instigator { get; set; }
            private float Level { get; set; } = 1;
            private Dictionary<string, int> CallerSuppliedModifierValues { get; set; } = new Dictionary<string, int>();

            private Builder(IAttributeReader instigator) {
                this.Instigator = instigator;
            }
            
            public static Builder From(IAttributeReader instigator) {
                return new Builder(instigator);
            }
            
            
            public Builder WithLevel(float level) {
                this.Level = level;
                return this;
            }

            /// <summary>
            /// Set a modifier value that will be used by the gameplay effect.
            /// </summary>
            /// <param name="magnitude">The modifier's magnitude.</param>
            /// <param name="label">The label of the modifier.
            /// It must match the label of the modifier in the gameplay effect.</param>
            /// <returns>The execution argument builder.</returns>
            public Builder WithModifier(int magnitude, string label) {
                this.CallerSuppliedModifierValues[label] = magnitude;
                return this;
            }
            
            public GameplayEffectExecutionArgs Build() {
                return new GameplayEffectExecutionArgs(this.Instigator, this.Level, this.CallerSuppliedModifierValues);
            }
        }
    }
}
