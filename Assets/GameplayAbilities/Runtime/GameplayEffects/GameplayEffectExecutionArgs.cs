using System.Collections.Generic;
using System.Collections.ObjectModel;
using GameplayAbilities.Runtime.Attributes;
using UnityEngine;

namespace GameplayAbilities.Runtime.GameplayEffects {
    public class GameplayEffectExecutionArgs {
        public IAttributeReader Instigator { get; }
        public Vector3 Position { get; }
        public float Level { get; }
        private IDictionary<string, object> CallerSuppliedDataValues { get; }

        private GameplayEffectExecutionArgs(
            IAttributeReader instigator, Vector3 position, float level, IDictionary<string, object> callerSuppliedDataValues
        ) {
            this.Instigator = instigator;
            this.Position = position;
            this.Level = level;
            this.CallerSuppliedDataValues = callerSuppliedDataValues;
        }
        
        public static Builder From(IAttributeReader instigator, Vector3 position) {
            return new Builder(instigator, position);
        }

        public bool HasData<T>(string label, out T data) {
            if (this.CallerSuppliedDataValues.TryGetValue(label, out object value) && value is T t) {
                data = t;
                return true;
            }

            Debug.LogWarning($"Custom data {label} not found!");
            data = default;
            return false;
        }

        public sealed class Builder {
            private IAttributeReader Instigator { get; set; }
            private float Level { get; set; } = 1;
            private Vector3 Position { get; set; } = Vector3.zero;

            private Dictionary<string, object> CallerSuppliedModifierValues { get; set; } =
                new Dictionary<string, object>();

            internal Builder(IAttributeReader instigator, Vector3 position) {
                this.Instigator = instigator;
                this.Position = position;           
            }

            public Builder At(Vector3 position) {
                this.Position = position;
                return this;           
            }
            
            public Builder WithLevel(float level) {
                this.Level = level;
                return this;
            }

            /// <summary>
            /// Set a custom data value that will be used by the gameplay effect.
            /// </summary>
            /// <param name="data">The custom data.</param>
            /// <param name="label">The label of the data entry.
            /// It must match the label of the modifier in the gameplay effect.</param>
            /// <returns>The execution argument builder.</returns>
            public Builder WithUserData<T>(string label, T data) {
                this.CallerSuppliedModifierValues[label] = data;
                return this;
            }
            
            public Builder WithUserData<T>(IEnumerable<KeyValuePair<string, T>> data) {
                if (data is null) {
                    return this;
                }
                
                foreach (KeyValuePair<string, T> pair in data) {
                    this.CallerSuppliedModifierValues[pair.Key] = pair.Value;
                }
                
                return this;
            }
            
            public GameplayEffectExecutionArgs Build() {
                return new GameplayEffectExecutionArgs(this.Instigator, this.Position, this.Level, this.CallerSuppliedModifierValues);
            }
        }
    }
}
