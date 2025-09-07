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
        private readonly InventoryUIManager _uiManager;
        private string _itemDescription;

        public SlotUI(VisualElement root, InventoryUIManager uiManager)
        {
            _root = root;
            _uiManager = uiManager;
            
            _iconElement = root.Q<VisualElement>("ItemIcon");
            _nameLabel = root.Q<Label>("ItemName");
            _quantityLabel = root.Q<Label>("ItemQuantity");
            
            // Hook hover events
            _root.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            _root.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
            _root.RegisterCallback<MouseMoveEvent>(OnMouseMove);
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
            
            // Set tooltip
            _itemDescription = itemData.Description;
            
        }

        private void OnMouseEnter(MouseEnterEvent e)
        { 
            _uiManager.ShowTooltip(_itemDescription, e.mousePosition);
        }

        private void OnMouseLeave(MouseLeaveEvent e)
        {
            _uiManager.HideTooltip();
        }

        private void OnMouseMove(MouseMoveEvent e)
        {
            _uiManager.ShowTooltip(_itemDescription, e.mousePosition);
        }

    }
}
