using UnityEngine;

namespace ModularItemsAndInventory.Runtime.Items {
    /// <summary>
    /// Represents a specific category or classification for items.
    /// Used to define item characteristics and metadata such as a default icon.
    /// Inherits from <see cref="ItemTypeDefinition"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "Item Type", menuName = "Modular Items and Inventory/Item Type")]
    public sealed class ItemType : ItemTypeDefinition {
        [field: SerializeField] public Sprite DefaultIcon { get; private set; }
    }
}
