using System;
using ModularItemsAndInventory.Runtime.Items.Properties;

namespace ModularItemsAndInventory.Runtime.Items {
    /// <summary>
    /// Represents an item with a specific type, name, and associated properties.
    /// This is a record type, which makes it immutable and suited for value-based equality.
    /// Implement <see cref="IComparable{T}"/> so that you can rank items in a list, for example.
    /// </summary>
    /// <param name="Type">The type of the item, represented by an <see cref="ItemType"/>.</param>
    /// <param name="Name">The name of the item.</param>
    /// <param name="Properties">The properties of the item, encapsulated by <see cref="ItemProperties"/>. Defaults to null if not provided.</param>
    public sealed record Item(ItemType Type, string Name, ItemProperties Properties = null) : IComparable<Item> {
        public Item(ItemData data) : this(data.Type, data.Name) {
            this.Properties = ItemProperties.Of(this).With(data.Properties);
        }
        
        public int CompareTo(Item other) {
            int comparison = this.Type.CompareTo(other.Type);
            if (comparison != 0) {
                return comparison;
            }
            
            comparison = string.CompareOrdinal(this.Name, other.Name);
            return comparison != 0 ? comparison : this.Properties.CompareTo(other.Properties);
        }
    }
}
