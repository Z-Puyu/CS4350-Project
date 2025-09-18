using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ModularItemsAndInventory.Runtime.Items;
using UnityEngine;
using UnityEngine.Events;

namespace ModularItemsAndInventory.Runtime.Inventory {
    /// <summary>
    /// Manages an inventory system that organises items into categories based on their type definitions
    /// and tracks the quantities of each item. Ensures constraints for what items can be stored and provides
    /// methods to add, remove, and query items.
    /// </summary>
    /// <remarks>
    /// The inventory groups items by <see cref="ItemType"/> and
    /// maintains a count for each specific <see cref="Item"/>.
    /// Supports operations such as adding, removing, counting, and querying items.
    /// </remarks>
    [DisallowMultipleComponent]
    public class Inventory : MonoBehaviour, IEnumerable<KeyValuePair<ItemKey, int>> {
        public enum OperationType { AddItem, RemoveItem }

        public readonly struct ItemOperation {
            public ItemKey Item { get; }
            public int OldQuantity { get; }
            public int CurrentQuantity { get; }
            public OperationType OperationType { get; }

            public int QuantityChange => this.CurrentQuantity - this.OldQuantity;

            public ItemOperation(ItemKey item, int oldQuantity, int currentQuantity, OperationType operationType) {
                this.Item = item;
                this.OldQuantity = oldQuantity;
                this.CurrentQuantity = currentQuantity;
                this.OperationType = operationType;
            }
        }

        [field: SerializeField] private ItemTypeDefinitionContext DefinedItemTypes { get; set; }

        private Dictionary<ItemType, Dictionary<ItemKey, int>> Items { get; set; } =
            new Dictionary<ItemType, Dictionary<ItemKey, int>>();
        
        private Dictionary<string, int> UniqueItems { get; set; } = new Dictionary<string, int>();

        public event UnityAction<ItemOperation> OnInventoryChanged;

        /// <summary>
        /// Provides indexer access to retrieve items of a specific type definition stored in the inventory.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> used to filter and retrieve associated items.</param>
        /// <returns>
        /// A dictionary of <see cref="Item"/> objects and their respective quantities
        /// for the specified item type definition.
        /// If no items of the given type are present, an empty dictionary is returned.
        /// </returns>
        public IReadOnlyDictionary<ItemKey, int> this[[NotNull] ItemType type] {
            get {
                if (this.Items.TryGetValue(type, out Dictionary<ItemKey, int> items)) {
                    return new ReadOnlyDictionary<ItemKey, int>(items);
                }

                return new Dictionary<ItemKey, int>();
            }
        }

        /// <summary>
        /// Retrieves the count of a specific item in the inventory.
        /// </summary>
        /// <param name="item">The item for which the count is to be retrieved.</param>
        /// <returns>The total number of the specified item in the inventory. Returns 0 if the item does not exist in the inventory.</returns>
        public int Count(ItemKey item) {
            return !this.Items.TryGetValue(ItemDatabase.TypeOf(item), out Dictionary<ItemKey, int> record)
                    ? 0
                    : record.GetValueOrDefault(item, 0);
        }

        /// <summary>
        /// Retrieves the total count of items with a specified type in the inventory.
        /// </summary>
        /// <param name="type">The type definition of the items to be counted.</param>
        /// <returns>The total count of items with the specified type in the inventory. Returns 0 if no items of that type are found.</returns>
        public int Count([NotNull] ItemType type) {
            return this.Items.TryGetValue(type, out Dictionary<ItemKey, int> record) ? record.Values.Sum() : 0;
        }
        
        /// <summary>
        /// Counts an item by its ID.
        /// </summary>
        /// <param name="id">The item ID.</param>
        /// <returns>The count of items with the ID.</returns>
        public int Count(string id) {
            return this.UniqueItems.GetValueOrDefault(id, 0);
        }

        /// <summary>
        /// Counts the total number of items that match a specified condition in the inventory.
        /// </summary>
        /// <param name="predicate">The condition to evaluate against each item in the inventory.</param>
        /// <returns>The total count of items that meet the specified condition.</returns>
        public int Count(Predicate<Item> predicate) {
            return this.Items.SelectMany(entry => entry.Value)
                       .Count(pair => ItemDatabase.TryGet(pair.Key, out Item item) && predicate(item));
        }

        /// <summary>
        /// Determines whether the specified item can be stored in the inventory based on its type.
        /// </summary>
        /// <param name="item">The item to be checked for storage eligibility.</param>
        /// <returns>true if the item can be stored in the inventory; otherwise, false.</returns>
        public bool CanStore(ItemKey item) {
            return this.DefinedItemTypes && this.DefinedItemTypes.Contains(ItemDatabase.TypeOf(item));
        }

        /// <summary>
        /// Adds a single copy of the specified item to the inventory.
        /// </summary>
        /// <param name="item">The item to be added to the inventory.</param>
        /// <returns>True if the item was successfully added to the inventory, otherwise false.</returns>
        public bool Add(ItemKey item) {
            return this.Add(1, item);
        }

        /// <summary>
        /// Adds the specified quantity of an item to the inventory.
        /// </summary>
        /// <param name="quantity">The quantity of the item to add. Must be greater than or equal to 1.</param>
        /// <param name="item">The item to add to the inventory.</param>
        /// <returns>True if the item was successfully added to the inventory; otherwise, false.</returns>
        public bool Add(int quantity, ItemKey item) {
            if (quantity < 1) {
                Debug.LogWarning("Minimally should add one copy of an item.", this);
                return false;
            }

            if (!this.CanStore(item)) {
                return false;
            }

            int oldQty = 0;
            int currQty;
            ItemType type = ItemDatabase.TypeOf(item);
            if (this.Items.TryGetValue(type, out Dictionary<ItemKey, int> record)) {
                oldQty = record.GetValueOrDefault(item, 0);
                currQty = oldQty + quantity;
                record[item] = currQty;
                this.UniqueItems[item.Id] = currQty;
            } else {
                currQty = quantity;
                this.Items.Add(type, new Dictionary<ItemKey, int> { { item, quantity } });
                this.UniqueItems.Add(item.Id, 1);
            }

            this.OnInventoryChanged?.Invoke(new ItemOperation(item, oldQty, currQty, OperationType.AddItem));
            return true;
        }

        /// <summary>
        /// Removes all instances of the specified item from the inventory.
        /// </summary>
        /// <param name="item">The item to remove from the inventory.</param>
        public void RemoveAll(ItemKey item) {
            if (!this.Items.TryGetValue(ItemDatabase.TypeOf(item), out Dictionary<ItemKey, int> record) ||
                !record.Remove(item, out int count)) {
                return;
            }

            this.UniqueItems[item.Id] -= count;
            if (this.UniqueItems[item.Id] <= 0) {
                this.UniqueItems.Remove(item.Id);
            }
                
            this.OnInventoryChanged?.Invoke(new ItemOperation(item, count, 0, OperationType.RemoveItem));
        }

        /// <summary>
        /// Removes a single instance of the specified item from the inventory.
        /// </summary>
        /// <param name="item">The item to be removed from the inventory. Must already exist in the inventory.</param>
        /// <returns>True if the item was successfully removed; otherwise,
        /// false if the item does not exist in the inventory or could not be removed.</returns>
        public bool Remove(ItemKey item) {
            return this.Remove(1, item);
        }

        /// <summary>
        /// Removes a specified quantity of an item from the inventory.
        /// </summary>
        /// <param name="quantity">The number of items to remove. Must be greater than zero.</param>
        /// <param name="item">The item to be removed from the inventory. Cannot be null.</param>
        /// <returns>
        /// True if the specified quantity of the item was successfully removed.
        /// Returns false if the item is null, the quantity is invalid, or the item does not exist
        /// in the required quantity within the inventory.
        /// </returns>
        public bool Remove(int quantity, ItemKey item) {
            if (quantity < 1) {
                Debug.LogWarning("Minimally should remove one copy of an item.", this);
                return false;
            }

            ItemType type = ItemDatabase.TypeOf(item);
            if (!this.Items.TryGetValue(type, out Dictionary<ItemKey, int> record) ||
                !record.TryGetValue(item, out int count)) {
                Debug.LogWarning($"Does not have any {item} to remove.", this);
                return false;
            }

            if (count < quantity) {
                Debug.LogWarning($"Trying to remove {quantity} copies of {item} but only has {count}.", this);
            }

            int remaining = record[item] = count - quantity;
            if (remaining <= 0) {
                record.Remove(item);
            }

            this.UniqueItems[item.Id] -= quantity;
            if (this.UniqueItems[item.Id] <= 0) {
                this.UniqueItems.Remove(item.Id);
            }
            
            this.OnInventoryChanged?.Invoke(new ItemOperation(item, count, remaining, OperationType.RemoveItem));
            return true;
        }

        /// <summary>
        /// Determines whether the inventory contains at least the specified quantity of the given item.
        /// </summary>
        /// <param name="quantity">The minimum quantity of the item to check for in the inventory.</param>
        /// <param name="item">The item whose availability is to be verified.</param>
        /// <returns>Returns true if the inventory contains at least the specified quantity of the item;
        /// otherwise, returns false.</returns>
        public bool ContainsAtLeast(int quantity, ItemKey item) {
            if (!this.Items.TryGetValue(ItemDatabase.TypeOf(item), out Dictionary<ItemKey, int> record)) {
                return false;
            }

            return record.TryGetValue(item, out int count) && count >= quantity;
        }

        public IEnumerator<KeyValuePair<ItemKey, int>> GetEnumerator() {
            return this.Items.SelectMany(record => record.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
