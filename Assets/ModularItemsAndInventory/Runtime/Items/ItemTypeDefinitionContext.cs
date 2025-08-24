using System.Collections.Generic;
using System.Linq;
using SaintsField;
using UnityEngine;

namespace ModularItemsAndInventory.Runtime.Items {
    /// <summary>
    /// Provides a context for managing and interacting with a collection of item type definitions in the modular items and inventory system.
    /// </summary>
    /// <remarks>
    /// This class serves as a container for item type definitions and categories, allowing for centralised management
    /// and lookup of defined item types. It also includes functionality to check
    /// if an item type definition belongs to the context.
    /// </remarks>
    [CreateAssetMenu(fileName = "Item Type Definition Context",
        menuName = "Modular Items and Inventory/Item Type Definition Context")]
    public sealed class ItemTypeDefinitionContext : ScriptableObject {
        [field: SerializeField]
        private List<ItemTypeDefinition> DefinedItemTypesAndCategories { get; set; } =
            new List<ItemTypeDefinition>();

        public bool Contains(ItemTypeDefinition definition) {
            return this.DefinedItemTypesAndCategories.Any(definition.BelongsTo);
        }
    }
}
