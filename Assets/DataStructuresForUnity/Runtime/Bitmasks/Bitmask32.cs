using System;

namespace DataStructuresForUnity.Runtime.Bitmasks {
    public struct Bitmask32 {
        private uint Value { get; set; }

        public Bitmask32(uint value) {
            this.Value = value;
        }

        public bool Has(uint mask) {
            return (this.Value & mask) == mask;
        }

        public void Set(int index) {
            if (index is < 0 or >= 32) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            this.Value |= 1u << index;
        }

        public void Unset(int index) {
            if (index is < 0 or >= 32) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            
            this.Value &= ~(1u << index);
        }

        public void Toggle(int index) {
            if (index is < 0 or >= 32) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            
            this.Value ^= 1u << index;
        }
        
        public static Bitmask32 operator |(Bitmask32 left, Bitmask32 right) {
            return new Bitmask32(left.Value | right.Value);
        }

        public static Bitmask32 operator &(Bitmask32 left, Bitmask32 right) {
            return new Bitmask32(left.Value & right.Value);
        }

        public static Bitmask32 operator ~(Bitmask32 value) {
            return new Bitmask32(~value.Value);
        }

        public static Bitmask32 operator ^(Bitmask32 left, Bitmask32 right) {
            return new Bitmask32(left.Value ^ right.Value);
        }

        public static implicit operator uint(Bitmask32 value) {
            return value.Value;
        }

        public static implicit operator Bitmask32(uint value) {
            return new Bitmask32(value);
        }

        public static implicit operator long(Bitmask32 value) {
            return value.Value;
        }
    }
}
