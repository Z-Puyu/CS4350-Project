using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items;
using UnityEngine;
using UnityEngine.UIElements;

namespace Inventory_related.Inventory_UI_Manager
{
    public class SlotUI
    {
        private readonly VisualElement _iconElement;
        private readonly Label _quantityLabel;
        private readonly InventoryUIManager _uiManager;
        
        private ItemKey _itemKey;
        private ItemData _itemData;

        public SlotUI(VisualElement root, InventoryUIManager uiManager)
        {
            _uiManager = uiManager;
            
            _iconElement = root.Q<VisualElement>("ItemIcon");
            _quantityLabel = root.Q<Label>("ItemQuantity");
            
            // Register click
            root.RegisterCallback<ClickEvent>(OnClick);
        }

        public void SetData(ItemKey itemKey, int quantity)
        {
            if (!ItemDatabase.TryGet(itemKey, out Item item)) return;
            if (!ItemDatabase.TryGet(item.Id, out ItemData itemData)) return;

            _itemKey = itemKey;
            _itemData = itemData;

            if (itemData.Icon != null)
                _iconElement.style.backgroundImage = new StyleBackground(itemData.Icon);

            _quantityLabel.text = quantity.ToString();
            Debug.Log($"SetData is Called with: key = {itemKey} & quantity = {quantity} & itemData = {_itemData}");
        }
        
        
        private void OnClick(ClickEvent evt)
        {
            _uiManager.UpdateItemPanel(_itemKey, _itemData);
        }

    }
}
