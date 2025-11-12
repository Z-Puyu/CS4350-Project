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
        private Label _itemName;
        private readonly VisualElement _root;
        private readonly BlacksmithUIManager _uiManager;

        private ItemKey _itemKey;
        private WeaponComponent _component;

        // Define the colors once using UnityEngine.Color or Color32
        private static readonly Color DefaultColor = new Color32(160, 82, 45, 255); // Sienna (A0522D)
        private static readonly Color EquippedColor = new Color32(0, 100, 0, 255); // Dark Green
        public BlacksmithSlotUI(VisualElement root, BlacksmithUIManager uiManager)
        {
            _uiManager = uiManager;

            _iconElement = root.Q<VisualElement>("ItemIcon");
            _itemName = root.Q<Label>("ItemName");
            _quantityLabel = root.Q<Label>("ItemQuantity");
            _priceLabel = root.Q<Label>("ItemPrice");
            var container = root.Q<VisualElement>("Container");
            var itemIcon = container.Q<VisualElement>("ItemIcon");

            // Register click
            root.RegisterCallback<ClickEvent>(OnClick);
        }

        public void SetData(string componentName, bool isEquipped)
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
            _itemName.text = component.ItemName;

            if (isEquipped)
            {
                // Set background to dark green
                _itemName.style.backgroundColor = new StyleColor(EquippedColor);
            }
            else
            {
                // Set background to sienna (A0522D)
                _itemName.style.backgroundColor = new StyleColor(DefaultColor);
            }

            Debug.Log($"SetData is Called with: key = {component.name}");
        }


        private void OnClick(ClickEvent evt)
        {
            _uiManager.UpdateItemPanel(_itemKey, _component);
        }
    }

}
