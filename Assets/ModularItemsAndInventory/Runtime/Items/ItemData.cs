using System.Collections.Generic;
using ModularItemsAndInventory.Runtime.Items.Properties;
using SaintsField;
using UnityEngine;

namespace ModularItemsAndInventory.Runtime.Items {
    /// <summary>
    /// Represents the data definition for an item in the game.
    /// Provides metadata such as the item type, name, description, and icon.
    /// Allows configuration and storage of additional item properties.
    /// </summary>
    [CreateAssetMenu(fileName = "Item Data", menuName = "Item/Data")]
    public sealed class ItemData : ScriptableObject {
        [field: SerializeField] public ItemType Type { get; private set; } 
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] private Sprite CustomIcon { get; set; }

        [field: SerializeReference, RichLabel(nameof(this.LabelProperty), true)]
        public List<IItemProperty> Properties { get; private set; } = new List<IItemProperty>();
        
        public Sprite Icon => this.CustomIcon ? this.CustomIcon : this.Type.DefaultIcon;

        private string LabelProperty(object prop, int _) => prop.GetType().Name;
    }
}
