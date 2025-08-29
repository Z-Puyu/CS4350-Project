using System;
using ModularItemsAndInventory.Runtime.Items.Properties;

namespace ModularItemsAndInventory.Runtime.Items {
    /// <summary>
    /// Lightweight value key for items to avoid heavy comparisons inside Inventory.
    /// Items with the same ID but different runtime names or properties will have different keys.
    /// Items that only share ID are still groupable by ID if needed.
    /// </summary>
    public readonly struct ItemKey : IEquatable<ItemKey>, IComparable<ItemKey> {
        public string Id { get; }
        private string Name { get; }
        private string EncodedProperties { get; }

        internal ItemKey(string id, string name, string encodedProperties) {
            this.Id = id;
            this.Name = name;
            this.EncodedProperties = encodedProperties;
        }

        public static ItemKey From(Item item) {
            return item.HasRuntimeData
                    ? new ItemKey(item.Id, string.Empty, string.Empty)
                    : new ItemKey(item.Id, item.Name, item.Properties.Encoding.Value);
        }

        public bool Equals(ItemKey other) {
            return this.CompareTo(other) == 0;
        }

        public override bool Equals(object obj) {
            return obj is ItemKey other && this.Equals(other);
        }

        public override int GetHashCode() {
            return HashCode.Combine(this.Id, this.Name, this.EncodedProperties);
        }

        public int CompareTo(ItemKey other) {
            int comp = string.CompareOrdinal(this.Id, other.Id);
            if (comp != 0) {
                return comp;
            }
            
            comp = string.CompareOrdinal(this.Name, other.Name);
            return comp != 0 ? comp : string.CompareOrdinal(this.EncodedProperties, other.EncodedProperties);
        }
        
        public static implicit operator string(ItemKey key) {
            return $"{key.Id}-{key.Name}-{key.EncodedProperties}";
        }
    }
}
