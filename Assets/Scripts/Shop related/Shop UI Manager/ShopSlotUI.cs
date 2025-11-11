using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items;
using UnityEngine;
using UnityEngine.UIElements;

namespace Shop_related.Shop_UI_Manager
{
    public class ShopSlotUI
    {
        private readonly VisualElement _iconElement;
        private readonly Label _quantityLabel;
        private readonly Label _priceLabel;
        private readonly ShopUIManager _uiManager;

        private ItemKey _itemKey;
        private ItemData _itemData;
        public ShopSlotUI(VisualElement root, ShopUIManager uiManager)
        {
            _uiManager = uiManager;

            _iconElement = root.Q<VisualElement>("ItemIcon");
            _quantityLabel = root.Q<Label>("ItemQuantity");
            _priceLabel = root.Q<Label>("ItemPrice");
            var container = root.Q<VisualElement>("Container");
            var itemIcon = container.Q<VisualElement>("ItemIcon");

            // Register click
            root.RegisterCallback<ClickEvent>(OnClick);
        }

        public void SetData(ItemKey itemKey, int quantity, int price)
        {
            if (!ItemDatabase.TryGet(itemKey, out Item item)) return;
            if (!ItemDatabase.TryGet(item.Id, out ItemData itemData)) return;

            _itemKey = itemKey;
            _itemData = itemData;

            if (itemData.Icon != null)
                _iconElement.style.backgroundImage = new StyleBackground(itemData.Icon);

            _quantityLabel.text = "X" + quantity.ToString();
            _priceLabel.text = "$" + price.ToString();
            Debug.Log($"SetData is Called with: key = {itemKey} & quantity = {quantity} & price = {price} & itemData = {_itemData}");
        }


        private void OnClick(ClickEvent evt)
        {
            _uiManager.UpdateItemPanel(_itemKey, _itemData);
        }
    }

}
