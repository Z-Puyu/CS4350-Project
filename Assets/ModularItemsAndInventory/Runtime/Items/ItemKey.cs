using System;
using System.Text;

namespace ModularItemsAndInventory.Runtime.Items {
    /// <summary>
    /// Lightweight value key for items to avoid heavy comparisons inside Inventory.
    /// Items with the same ID but different runtime names or properties will have different keys.
    /// Items that only share ID are still groupable by ID if needed.
    /// </summary>
    public readonly struct ItemKey : IEquatable<ItemKey>, IComparable<ItemKey> {
        public string Id { get; }
        internal string Name { get; }
        private string EncodedProperties { get; }

        internal ItemKey(string id, string name, string encodedProperties) {
            this.Id = id.Trim();
            this.Name = name.Trim();
            this.EncodedProperties = encodedProperties.Trim();
        }

        public static ItemKey From(Item item) {
            return item.HasRuntimeData
                    ? new ItemKey(item.Id, item.Name, item.Properties.Encoding.Value)
                    : new ItemKey(item.Id, string.Empty, string.Empty);
        }

        public static ItemKey From(ItemData data)
        {
            return new ItemKey(data.Id, string.Empty, string.Empty);
        }
        
        public static ItemKey FromID(string id) {
            return new ItemKey(id, string.Empty, string.Empty);
        }

        public override string ToString() {
            return $"{this.Id}{(string.IsNullOrWhiteSpace(this.Name) ? string.Empty : $"({this.Name})")}";
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
            StringBuilder sb = new StringBuilder(key.Id);
            if (!string.IsNullOrWhiteSpace(key.Name)) {
                sb.Append('-').Append(key.Name);
            }

            if (!string.IsNullOrWhiteSpace(key.EncodedProperties)) {
                sb.Append('-').Append(key.EncodedProperties);
            }
            
            return sb.ToString();
        }
    }
}
