using UnityEngine;

namespace ModularItemsAndInventory.Runtime.Items {
    /// <summary>
    /// Represents a specific category for items within the modular items and inventory system.
    /// </summary>
    /// <remarks>
    /// This class is used to define and organise items into categories, allowing for hierarchical
    /// categorisation and grouping of items. It inherits from the <see cref="ItemTypeDefinition"/> base class
    /// to provide shared functionality such as name management and category assignments.
    /// </remarks>
    [CreateAssetMenu(fileName = "Item Category", menuName = "Modular Items and Inventory/Item Category")]
    public sealed class ItemCategory : ItemTypeDefinition { }
}
