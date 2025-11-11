using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items;
using UnityEngine;
using UnityEngine.UIElements;
using WeaponsSystem.Runtime.WeaponComponents;

namespace Shop_related.Shop_UI_Manager
{
    public class BlacksmithSlotUI
    {
        private readonly VisualElement _iconElement;
        private readonly Label _quantityLabel;
        private readonly Label _priceLabel;
        private Label _itemDescription;
        private readonly BlacksmithUIManager _uiManager;

        private ItemKey _itemKey;
        private WeaponComponent _component;
        public BlacksmithSlotUI(VisualElement root, BlacksmithUIManager uiManager)
        {
            _uiManager = uiManager;

            _iconElement = root.Q<VisualElement>("ItemIcon");
            _itemDescription = root.Q<Label>("ItemDescription");
            _quantityLabel = root.Q<Label>("ItemQuantity");
            _priceLabel = root.Q<Label>("ItemPrice");
            var container = root.Q<VisualElement>("Container");
            var itemIcon = container.Q<VisualElement>("ItemIcon");

            // Register click
            root.RegisterCallback<ClickEvent>(OnClick);
        }

        public void SetData(string componentName)
        {
            if (!ComponentDatabase.TryGet(componentName, out WeaponComponent component))
            {
                Debug.LogWarning($"Weapon component not found: {componentName}");
                return;
            }

            _component = component;
            _itemKey = component.GetItemKey();

            if (component.icon != null)
                _iconElement.style.backgroundImage = new StyleBackground(component.icon);
            _itemDescription.text = component.Description;

            // _quantityLabel.text = "X" + quantity.ToString();
            // _priceLabel.text = "$" + price.ToString();
            Debug.Log($"SetData is Called with: key = {component.name}");
        }


        private void OnClick(ClickEvent evt)
        {
            _uiManager.UpdateItemPanel(_itemKey, _component);
        }
    }

}
