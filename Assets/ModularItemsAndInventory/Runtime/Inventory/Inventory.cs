using System;
using System.Collections;
using System.Collections.Generic;
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
    /// The inventory groups items by <see cref="ItemTypeDefinition"/> and
    /// maintains a count for each specific <see cref="Item"/>.
    /// Supports operations such as adding, removing, counting, and querying items.
    /// </remarks>
    [DisallowMultipleComponent]
    public class Inventory : MonoBehaviour, IEnumerable<KeyValuePair<Item, int>> {
        public enum OperationType { AddItem, RemoveItem }
        
        public struct ItemOperation {
            public Item Item { get; }
            public int OldQuantity { get; }
            public int CurrentQuantity { get; }
            public OperationType OperationType { get; }
            
            public ItemOperation(Item item, int oldQuantity, int currentQuantity, OperationType operationType) {
                this.Item = item;
                this.OldQuantity = oldQuantity;
                this.CurrentQuantity = currentQuantity;
                this.OperationType = operationType;
            }
        }
        
        [field: SerializeField] private ItemTypeDefinitionContext DefinedItemTypes { get; set; }

        private Dictionary<ItemTypeDefinition, Dictionary<Item, int>> Items { get; set; } =
            new Dictionary<ItemTypeDefinition, Dictionary<Item, int>>();
        
        public event UnityAction<ItemOperation> OnInventoryChanged;

        /// <summary>
        /// Provides indexer access to retrieve items of a specific type definition stored in the inventory.
        /// </summary>
        /// <param name="type">The <see cref="ItemTypeDefinition"/> used to filter and retrieve associated items.</param>
        /// <returns>
        /// A dictionary of <see cref="Item"/> objects and their respective quantities
        /// for the specified item type definition.
        /// If no items of the given type are present, an empty dictionary is returned.
        /// </returns>
        public IDictionary<Item, int> this[[NotNull] ItemTypeDefinition type] =>
                this.Items.TryGetValue(type, out Dictionary<Item, int> items)
                        ? items
                        : new Dictionary<Item, int>();

        /// <summary>
        /// Retrieves the count of a specific item in the inventory.
        /// </summary>
        /// <param name="item">The item for which the count is to be retrieved.</param>
        /// <returns>The total number of the specified item in the inventory. Returns 0 if the item does not exist in the inventory.</returns>
        public int Count([NotNull] Item item) {
            return this.Items.TryGetValue(item.Type, out Dictionary<Item, int> record)
                    ? record.GetValueOrDefault(item, 0)
                    : 0;
        }

        /// <summary>
        /// Retrieves the total count of items with a specified type in the inventory.
        /// </summary>
        /// <param name="type">The type definition of the items to be counted.</param>
        /// <returns>The total count of items with the specified type in the inventory. Returns 0 if no items of that type are found.</returns>
        public int Count([NotNull] ItemTypeDefinition type) {
            return this.Items.TryGetValue(type, out Dictionary<Item, int> record) ? record.Values.Sum() : 0;
        }

        /// <summary>
        /// Counts the total number of items that match a specified condition in the inventory.
        /// </summary>
        /// <param name="predicate">The condition to evaluate against each item in the inventory.</param>
        /// <returns>The total count of items that meet the specified condition.</returns>
        public int Count(Predicate<Item> predicate) {
            return this.Items.SelectMany(entry => entry.Value)
                       .Count(record => predicate(record.Key));
        }

        /// <summary>
        /// Determines whether the specified item can be stored in the inventory based on its type.
        /// </summary>
        /// <param name="item">The item to be checked for storage eligibility.</param>
        /// <returns>true if the item can be stored in the inventory; otherwise, false.</returns>
        public bool CanStore([NotNull] Item item) {
            return this.DefinedItemTypes && this.DefinedItemTypes.Contains(item.Type);
        }

        /// <summary>
        /// Adds a single copy of the specified item to the inventory.
        /// </summary>
        /// <param name="item">The item to be added to the inventory.</param>
        /// <returns>True if the item was successfully added to the inventory, otherwise false.</returns>
        public bool Add([NotNull] Item item) {
            return this.Add(1, item);
        }

        /// <summary>
        /// Adds the specified quantity of an item to the inventory.
        /// </summary>
        /// <param name="quantity">The quantity of the item to add. Must be greater than or equal to 1.</param>
        /// <param name="item">The item to add to the inventory.</param>
        /// <returns>True if the item was successfully added to the inventory; otherwise, false.</returns>
        public bool Add(int quantity, [NotNull] Item item) {
            if (quantity < 1) {
                Debug.LogWarning("Minimally should add one copy of an item.", this);
                return false;
            }

            if (!this.CanStore(item)) {
                return false;
            }

            int oldQty = 0;
            int currQty;
            ItemTypeDefinition type = item.Type;
            if (this.Items.TryGetValue(type, out Dictionary<Item, int> record)) {
                oldQty = record.GetValueOrDefault(item, 0);
                currQty = record[item] = oldQty + quantity;
            } else {
                currQty = quantity;
                this.Items.Add(type, new Dictionary<Item, int> { { item, quantity } });
            }

            this.OnInventoryChanged?.Invoke(new ItemOperation(item, oldQty, currQty, OperationType.AddItem));
            return true;
        }

        /// <summary>
        /// Removes all instances of the specified item from the inventory.
        /// </summary>
        /// <param name="item">The item to remove from the inventory.</param>
        public void RemoveAll([NotNull] Item item) {
            if (this.Items.TryGetValue(item.Type, out Dictionary<Item, int> record) && record.Remove(item, out int count)) {
                this.OnInventoryChanged?.Invoke(new ItemOperation(item, count, 0, OperationType.RemoveItem));
            }
        }

        /// <summary>
        /// Removes a single instance of the specified item from the inventory.
        /// </summary>
        /// <param name="item">The item to be removed from the inventory. Must already exist in the inventory.</param>
        /// <returns>True if the item was successfully removed; otherwise,
        /// false if the item does not exist in the inventory or could not be removed.</returns>
        public bool Remove([NotNull] Item item) {
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
        public bool Remove(int quantity, [NotNull] Item item) {
            if (item is null) {
                Debug.LogError("Cannot remove null item to inventory.", this);
                return false;
            }
            
            if (quantity < 1) {
                Debug.LogWarning("Minimally should remove one copy of an item.", this);
                return false;
            }
            
            ItemTypeDefinition type = item.Type;
            if (!this.Items.TryGetValue(type, out Dictionary<Item, int> record) ||
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
        public bool ContainsAtLeast(int quantity, [NotNull] Item item) {
            if (!this.Items.TryGetValue(item.Type, out Dictionary<Item, int> record)) {
                return false;
            }
            
            return record.TryGetValue(item, out int count) && count >= quantity;
        }

        public IEnumerator<KeyValuePair<Item, int>> GetEnumerator() {
            return this.Items.SelectMany(record => record.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
