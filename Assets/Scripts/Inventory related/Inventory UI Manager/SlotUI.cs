using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items;
using UnityEngine;
using UnityEngine.UIElements;

namespace Inventory_related.Inventory_UI_Manager
{
    public class SlotUI : MonoBehaviour
    {
        private readonly VisualElement _root;
        private readonly VisualElement _iconElement;
        private readonly Label _nameLabel;
        private readonly Label _quantityLabel;

        public SlotUI(VisualElement root)
        {
            _root = root;
            _iconElement = root.Q<VisualElement>("ItemIcon");
            _nameLabel = root.Q<Label>("ItemName");
            _quantityLabel = root.Q<Label>("ItemQuantity");
        }

        public void SetData(ItemKey itemKey, int quantity)
        {
            // Resolve runtime item
            if (!ItemDatabase.TryGet(itemKey, out Item item))
            {
                Debug.LogWarning($"Item {itemKey} not found in Database");
                return;
            }
            
            // Get Definition
            if (!ItemDatabase.TryGet(item.Id, out ItemData itemData))
            {
                Debug.LogWarning($"Item {item.Id} not found in Database");
            }
            
            // Set Icon
            if (itemData.Icon != null)
            {
                _iconElement.style.backgroundImage = new StyleBackground(itemData.Icon);
            }
            
            // Set Name
            _nameLabel.text = itemData.Name;
            
            // Set Quantity
            _quantityLabel.text = quantity.ToString();
        }

    }
}
