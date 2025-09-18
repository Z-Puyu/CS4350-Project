using System.Collections.Generic;
using System.Collections.ObjectModel;
using GameplayAbilities.Runtime.Attributes;

namespace GameplayAbilities.Runtime.GameplayEffects {
    public class GameplayEffectExecutionArgs {
        public IAttributeReader Instigator { get; }
        public float Level { get; }
        public IReadOnlyDictionary<string, int> CallerSuppliedDataValues { get; }

        private GameplayEffectExecutionArgs(
            IAttributeReader instigator, float level, IDictionary<string, int> callerSuppliedDataValues
        ) {
            this.Instigator = instigator;
            this.Level = level;
            this.CallerSuppliedDataValues = new ReadOnlyDictionary<string, int>(callerSuppliedDataValues);
        }
        
        public static Builder From(IAttributeReader instigator) {
            return new Builder(instigator);
        }

        public sealed class Builder {
            private IAttributeReader Instigator { get; set; }
            private float Level { get; set; } = 1;
            private Dictionary<string, int> CallerSuppliedModifierValues { get; set; } = new Dictionary<string, int>();

            internal Builder(IAttributeReader instigator) {
                this.Instigator = instigator;
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
            
            public Builder WithModifiers(IEnumerable<KeyValuePair<string, int>> modifiers) {
                foreach (KeyValuePair<string, int> modifier in modifiers) {
                    this.CallerSuppliedModifierValues[modifier.Key] = modifier.Value;
                }
                
                return this;
            }
            
            public GameplayEffectExecutionArgs Build() {
                return new GameplayEffectExecutionArgs(this.Instigator, this.Level, this.CallerSuppliedModifierValues);
            }
        }
    }
}
