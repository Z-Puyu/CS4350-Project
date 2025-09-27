using System;

namespace DataStructuresForUnity.Runtime.Bitmasks {
    public struct Bitmask64 {
        private ulong Value { get; set; }

        public Bitmask64(ulong value) {
            this.Value = value;
        }

        public bool Has(ulong mask) {
            return (this.Value & mask) == mask;
        }

        public void Set(int index) {
            if (index is < 0 or >= 64) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            this.Value |= 1u << index;
        }

        public void Unset(int index) {
            if (index is < 0 or >= 64) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            
            this.Value &= ~(1u << index);
        }

        public void Toggle(int index) {
            if (index is < 0 or >= 64) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            
            this.Value ^= 1u << index;
        }
        
        public static Bitmask64 operator |(Bitmask64 left, Bitmask64 right) {
            return new Bitmask64(left.Value | right.Value);
        }

        public static Bitmask64 operator &(Bitmask64 left, Bitmask64 right) {
            return new Bitmask64(left.Value & right.Value);
        }

        public static Bitmask64 operator ~(Bitmask64 value) {
            return new Bitmask64(~value.Value);
        }

        public static Bitmask64 operator ^(Bitmask64 left, Bitmask64 right) {
            return new Bitmask64(left.Value ^ right.Value);
        }

        public static implicit operator ulong(Bitmask64 value) {
            return value.Value;
        }

        public static implicit operator Bitmask64(ulong value) {
            return new Bitmask64(value);
        }
    }
}
