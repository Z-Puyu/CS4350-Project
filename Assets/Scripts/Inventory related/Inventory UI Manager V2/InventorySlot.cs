using TMPro;
using UnityEngine;
using ModularItemsAndInventory.Runtime.Items;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

namespace Inventory_related.Inventory_UI_Manager_V2
{
    public class InventorySlot : MonoBehaviour
    {
        [SerializeField] private Image itemImage;        // Assign in inspector
        [SerializeField] private TextMeshProUGUI quantityText;
        [SerializeField] private GameObject quickSwapIcon;
        [SerializeField] private Button slotButton;
        
        private ItemData _itemData;
        private ItemKey _itemKey;
        private InventoryUIManagerV2 _inventoryUIManager;
        
        public void Initialize(InventoryUIManagerV2 manager)
        {
            _inventoryUIManager = manager;
        }

        public void SetData(ItemData item, ItemKey itemKey, int quantity, bool isInQuickSwap)
        {
            _itemData = item;
            _itemKey =  itemKey;
            
            if (itemImage != null && _itemData.Icon != null)
                itemImage.sprite = item.Icon;

            if (quantityText != null)
                quantityText.text = quantity > 1 ? quantity.ToString() : "";

            if (quickSwapIcon != null)
                quickSwapIcon.SetActive(isInQuickSwap);
            
            // Make sure button calls back to manager
            if (slotButton != null)
            {
                slotButton.onClick.RemoveAllListeners();
                slotButton.onClick.AddListener(() => _inventoryUIManager.OnSlotClicked(_itemData, _itemKey));
            }
        }
        
        public void HandleItemForQuickSwap(bool isInQuickSwap)
        {
            if (quickSwapIcon != null)
            {
                quickSwapIcon.SetActive(isInQuickSwap);   
            }
        }
    }
}