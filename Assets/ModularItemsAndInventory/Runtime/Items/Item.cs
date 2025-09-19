using System;
using System.Collections.Generic;
using System.Linq;
using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items.Properties;
using UnityEngine;

namespace ModularItemsAndInventory.Runtime.Items {
    /// <summary>
    /// Represents an item with a specific type, name, and associated properties.
    /// This is a record type, which makes it immutable and suited for value-based equality.
    /// Implement <see cref="IComparable{T}"/> so that you can rank items in a list, for example.
    /// </summary>
    /// <param name="HasRuntimeData">Whether the item data has been modified during runtime.</param>   
    /// <param name="Id">The unique identifier of the item.</param>
    /// <param name="Type">The type of the item, represented by an <see cref="ItemType"/>.</param>
    /// <param name="Name">The name of the item.</param>
    /// <param name="Properties">The properties of the item, encapsulated by <see cref="ItemProperties"/>. Defaults to null if not provided.</param>
    public sealed record Item(
        bool HasRuntimeData,
        string Id,
        ItemType Type,
        string Name,
        string Description,
        ItemProperties Properties = null
    ) : IComparable<Item> {
        private Lazy<ItemKey> CachedKey { get; }
        public ItemKey Key => this.CachedKey.Value;
        
        public static Item From(ItemData data) {
            return new Item(false, data.Id, data.Type, data.Name, data.Description, data.Properties);
        }

        public Item Duplicate() {
            return new Item(this.HasRuntimeData, this.Id, this.Type, this.Name, this.Description, this.Properties);
        }

        public Item Duplicate(string newName) {
            ItemKey newKey = new ItemKey(this.Id, newName, this.Properties.Encoding.Value);
            return new Item(ItemDatabase.IsRuntimeDefined(newKey), this.Id, this.Type, newName, this.Description, this.Properties);
        }

        public Item Duplicate(IEnumerable<IItemProperty> newProperties) {
            Dictionary<Type, IItemProperty> properties = this.Properties.ToDictionary(p => p.GetType());
            foreach (IItemProperty property in newProperties) {
                properties[property.GetType()] = property;
            }

            ItemKey newKey = new ItemKey(this.Id, this.Name, ItemProperties.Encode(properties.Values));
            return new Item(ItemDatabase.IsRuntimeDefined(newKey), this.Id, this.Type, this.Name, this.Description, properties.Values);
        }

        public Item Duplicate(string newName, IEnumerable<IItemProperty> newProperties) {
            Dictionary<Type, IItemProperty> properties = this.Properties.ToDictionary(p => p.GetType());
            foreach (IItemProperty property in newProperties) {
                properties[property.GetType()] = property;
            }
            
            ItemKey newKey = new ItemKey(this.Id, newName, ItemProperties.Encode(properties.Values));
            return new Item(ItemDatabase.IsRuntimeDefined(newKey), this.Id, this.Type, newName, this.Description, properties.Values);
        }

        private Item(
            bool hasRuntimeData, string id, ItemType type, string name, string description,
            IEnumerable<IItemProperty> properties
        ) : this(hasRuntimeData, id, type, name, description) {
            this.Properties = ItemProperties.Of(this).With(properties);
            this.CachedKey = new Lazy<ItemKey>(this.GenerateKey);
        }

        public PickUp2D AsPickUp(int count = 1) {
            GameObject pickUp = new GameObject("Pick-up: " + this.Name);
            pickUp.transform.position = Vector3.zero;
            return pickUp.AddComponent<PickUp2D>().With(count, this.Key);
        }

        private ItemKey GenerateKey() {
            return this.HasRuntimeData ? ItemKey.From(this) : new ItemKey(this.Id, string.Empty, string.Empty);
        }

        public int CompareTo(Item other) {
            int comparison = string.CompareOrdinal(this.Id, other.Id);
            if (comparison != 0) {
                return comparison;
            }

            comparison = this.Type.CompareTo(other.Type);
            return comparison != 0 ? comparison : this.Properties.CompareTo(other.Properties);
        }
    }
}
