using System.Collections.Generic;
using System.Collections.ObjectModel;
using GameplayAbilities.Runtime.Attributes;
using UnityEngine;

namespace GameplayAbilities.Runtime.GameplayEffects {
    public class GameplayEffectExecutionArgs {
        public IAttributeReader Instigator { get; }
        public Transform FromTransform { get; }
        public Transform TargetTransform { get; }
        private Vector3 ToPosition { get; }
        public Vector3 TargetPosition => this.TargetTransform ? this.TargetTransform.position : this.ToPosition;
        public float Level { get; }
        private IDictionary<string, object> CallerSuppliedDataValues { get; }

        private GameplayEffectExecutionArgs(
            IAttributeReader instigator, Transform from, Transform target, Vector3 toPosition, float level,
            IDictionary<string, object> callerSuppliedDataValues
        ) {
            this.Instigator = instigator;
            this.FromTransform = from;
            this.Level = level;
            this.CallerSuppliedDataValues = callerSuppliedDataValues;
            this.TargetTransform = target;
            this.ToPosition = toPosition;
        }

        public static Builder From(IAttributeReader instigator, Transform from) {
            return new Builder(instigator, from);
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
            private Transform FromTransform { get; set; }
            private Transform TargetTransform { get; set; }
            private Vector3 ToPosition { get; set; }

            private Dictionary<string, object> CallerSuppliedModifierValues { get; set; } =
                new Dictionary<string, object>();

            internal Builder(IAttributeReader instigator, Transform fromTransform) {
                this.Instigator = instigator;
                this.FromTransform = fromTransform;
            }

            public Builder From(Transform transform) {
                this.FromTransform = transform;
                return this;
            }

            public Builder To(Transform transform) {
                this.TargetTransform = transform;
                return this;
            }

            public Builder To(Vector3 position) {
                this.ToPosition = position;
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
                return new GameplayEffectExecutionArgs(
                    this.Instigator, this.FromTransform, this.TargetTransform, this.ToPosition, this.Level,
                    this.CallerSuppliedModifierValues
                );
            }
        }
    }
}
