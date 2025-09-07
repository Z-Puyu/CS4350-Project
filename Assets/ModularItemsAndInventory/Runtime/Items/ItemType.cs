using System;
using System.Text;
using UnityEngine;

namespace ModularItemsAndInventory.Runtime.Items {
    /// <summary>
    /// Represents a base definition for item types within the modular items and inventory system.
    /// Serves as an abstract base class for specific item types and categories.
    /// </summary>
    /// <remarks>
    /// This class provides shared functionality to item types and categories,
    /// including name, category assignments, and comparison logic.
    /// </remarks>
    [CreateAssetMenu(fileName = "New Item Type", menuName = "Modular Items and Inventory/Item Type")]
    public class ItemType : ScriptableObject, IComparable<ItemType> {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public ItemType Category { get; private set; }
        [field: SerializeField] public Sprite DefaultIcon { get; private set; }
        private string FullName { get; set; }

        public override string ToString() {
            return this.FullName;
        }

        public bool BelongsTo(ItemType type) {
            ItemType curr = this;
            while (curr) {
                if (curr == type) {
                    return true;
                }
                
                curr = curr.Category;
            }

            return false;
        }

        private void OnValidate() {
            StringBuilder sb = new StringBuilder(this.Name);
            ItemType category = this.Category;
            while (category) {
                sb.Insert(0, $"{category.Name}.");
                category = category.Category;
            }

            this.FullName = sb.ToString();
        }

        public int CompareTo(ItemType other) {
            return this == other ? 0 : string.CompareOrdinal(this.FullName, other.FullName);
        }
    }
}
