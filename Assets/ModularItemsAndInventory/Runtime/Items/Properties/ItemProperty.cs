using System;

namespace ModularItemsAndInventory.Runtime.Items.Properties {
    [Serializable]
    public abstract class ItemProperty : IItemProperty {
        public bool Equals(IItemProperty other) {
            return this.CompareTo(other) == 0;
        }
        
        public int CompareTo(IItemProperty other) {
            if (object.ReferenceEquals(this, other)) {
                return 0;
            }

            if (other is null) {
                return 1;
            }
            
            int comparison = string.CompareOrdinal(this.GetType().FullName, other.GetType().FullName);
            return comparison == 0 ? this.CompareToSameType(other) : comparison;
        }
        
        /// <summary>
        /// Compare this instance with another instance of the exact same type.
        /// </summary>
        /// <param name="otherOfSameType">The other instance of the exact same type.</param>
        /// <returns>A positive integer if this instance is greater, a negative integer if
        /// the other instance is greater, and zero if they are equal.</returns>
        protected abstract int CompareToSameType(IItemProperty otherOfSameType);

        public abstract IItemProperty Instantiate();
    }
    
    [Serializable]
    public abstract class ItemProperty<T> : IItemProperty<T> {
        public int CompareTo(IItemProperty other) {
            if (object.ReferenceEquals(this, other)) {
                return 0;
            }

            if (other is null) {
                return 1;
            }
            
            int comparison = string.CompareOrdinal(this.GetType().FullName, other.GetType().FullName);
            return comparison == 0 ? this.CompareToSameType(other) : comparison;
        }

        public abstract IItemProperty Instantiate();

        /// <summary>
        /// Compare this instance with another instance of the exact same type.
        /// </summary>
        /// <param name="otherOfSameType">The other instance of the exact same type.</param>
        /// <returns>A positive integer if this instance is greater, a negative integer if
        /// the other instance is greater, and zero if they are equal.</returns>
        protected abstract int CompareToSameType(IItemProperty otherOfSameType);

        public abstract void Process(in Item item, T target);

        public sealed override bool Equals(object obj) {
            return this.Equals(obj as IItemProperty);
        }

        public bool Equals(IItemProperty other) {
            return this.CompareTo(other) == 0;
        }

        public sealed override int GetHashCode() {
            return HashCode.Combine(this.GetType(), this.HashPropertyContents());
        }

        /// <summary>
        /// Concrete types must return a stable hash code for their fields used in comparison.
        /// </summary>
        /// <returns>A hash code evaluated from the fields used in comparison.</returns>
        protected abstract int HashPropertyContents();
    }
}
