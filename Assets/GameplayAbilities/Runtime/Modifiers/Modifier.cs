using System;
using UnityEngine;

namespace GameplayAbilities.Runtime.Modifiers {
    /// <summary>
    /// Represents a modification that can be applied to a game attribute.
    /// Supports different types of operations (Shift, Multiply, Offset) and can be combined using arithmetic operators.
    /// </summary>
    public readonly struct Modifier : IEquatable<Modifier> {
        /// <summary>
        /// Defines the types of operations that can be performed on an attribute value.
        /// </summary>
        public enum Operation {
            /// <summary>Adds or subtracts from the base value (flat modification)</summary>
            Shift,

            /// <summary>
            /// Multiplies the base value by a percentage (e.g., 10 = 110% of base)
            /// </summary>
            /// <remarks>
            /// Multipliers are added before applied to the base value,
            /// i.e., two +10% multipliers are composed to +20% instead of +21%.
            /// </remarks>
            Multiply,

            /// <summary>Adds or subtracts from the final value (applied after all other modifications)</summary>
            Offset
        }
        
        public Operation Type { get; }
        public string Target { get; }
        private int Magnitude { get; }
        
        public Modifier(int magnitude, Operation type, string target) {
            this.Type = type;
            this.Target = target;
            this.Magnitude = magnitude;
        }

        /// <summary>
        /// Applies the modifier to the given value.
        /// </summary>
        /// <param name="value">The original value to modify.</param>
        /// <returns>The modified value after applying this modifier.</returns>
        public double Modify(double value) {
            return this.Type switch {
                Operation.Shift or Operation.Offset => value + this.Magnitude,
                Operation.Multiply => value * Math.Max(100 + this.Magnitude, 0) / 100.0f,
                var _ => value
            };
        }
        
        public override string ToString() {
            return $"{this.Target} {this.Type} of magnitude {this.Magnitude}";
        }

        /// <summary>
        /// Negates the magnitude of a modifier.
        /// </summary>
        /// <param name="m">The modifier to negate.</param>
        /// <returns>A new modifier with negated magnitude.</returns>
        public static Modifier operator -(Modifier m) {
            return new Modifier(-m.Magnitude, m.Type, m.Target);
        }

        /// <summary>
        /// Adds two modifiers of the same type and target.
        /// </summary>
        /// <param name="a">The first modifier.</param>
        /// <param name="b">The second modifier.</param>
        /// <returns>A new modifier with combined magnitudes.</returns>
        /// <exception cref="ArgumentException">Thrown when modifiers have different targets or types.</exception>
        public static Modifier operator +(Modifier a, Modifier b) {
            if (a.Target != b.Target || a.Type != b.Type) {
                throw new ArgumentException("Cannot add modifiers with different targets or types");
            }

            return new Modifier(a.Magnitude + b.Magnitude, a.Type, a.Target);
        }

        /// <summary>
        /// Subtracts one modifier from another of the same type and target.
        /// </summary>
        /// <param name="a">The minuend modifier.</param>
        /// <param name="b">The subtrahend modifier.</param>
        /// <returns>A new modifier with the difference in magnitudes.</returns>
        /// <exception cref="ArgumentException">Thrown when modifiers have different targets or types.</exception>
        public static Modifier operator -(Modifier a, Modifier b) {
            if (a.Target != b.Target || a.Type != b.Type) {
                throw new ArgumentException("Cannot add modifiers with different targets or types");
            }

            return new Modifier(a.Magnitude - b.Magnitude, a.Type, a.Target);
        }

        /// <summary>
        /// Multiplies a modifier's magnitude by a scalar value.
        /// </summary>
        /// <param name="a">The modifier to scale.</param>
        /// <param name="k">The scalar value to multiply by.</param>
        /// <returns>A new modifier with scaled magnitude.</returns>
        public static Modifier operator *(Modifier a, float k) {
            return new Modifier(Mathf.RoundToInt(k * a.Magnitude), a.Type, a.Target);
        }

        public static Modifier operator *(float k, Modifier a) {
            return a * k;
        }

        /// <summary>
        /// Divides a modifier's magnitude by a scalar value.
        /// </summary>
        /// <param name="a">The modifier to scale.</param>
        /// <param name="k">The scalar value to divide by.</param>
        /// <returns>A new modifier with scaled magnitude.</returns>
        public static Modifier operator /(Modifier a, float k) {
            return new Modifier(Mathf.RoundToInt(a.Magnitude / k), a.Type, a.Target);
        }

        public bool Equals(Modifier other) {
            return this.Type == other.Type && this.Magnitude == other.Magnitude && this.Target == other.Target;
        }

        public override bool Equals(object obj) {
            return obj is Modifier other && this.Equals(other);
        }

        public override int GetHashCode() {
            return HashCode.Combine((int)this.Type, this.Target, this.Magnitude);
        }
    }
}
