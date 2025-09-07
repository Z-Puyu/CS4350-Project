using System;
using ModularItemsAndInventory.Runtime.Inventory;
using UnityEngine;
using UnityEngine.UIElements;

namespace Inventory_related.Inventory_UI_Manager
{
    public class InventoryUIManager : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private VisualTreeAsset slotTemplate; // assign Slot.uxml in inspector
        [SerializeField] private Inventory inventory;
        [SerializeField] private VisualTreeAsset tooltipTemplate;

        private VisualElement _root;
        private VisualElement _grid;
        private VisualElement _tooltip;
        private Label _tooltipText;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _root = uiDocument.rootVisualElement;
            _grid = _root.Q<VisualElement>("InventoryGrid");
            
            // Tooltip
            _tooltip = tooltipTemplate.CloneTree();
            // _tooltip.style.visibility = Visibility.Hidden;
            _tooltipText = _tooltip.Q<Label>("TooltipText");
            
            _root.Add(_tooltip);

            // Listen to inventory changes
            inventory.OnInventoryChanged += HandleInventoryChanged;

            // Initial draw
            RefreshInventoryUI();
        }

        private void OnDisable()
        {
            inventory.OnInventoryChanged -= HandleInventoryChanged;
        }

        private void HandleInventoryChanged(Inventory.ItemOperation operation)
        {
            RefreshInventoryUI();
        }

        public void ShowTooltip(string text, Vector2 mousePosition)
        {
            _tooltipText.text = text;
            
            _tooltip.style.left = mousePosition.x;
            _tooltip.style.top = mousePosition.y;
            _tooltip.style.visibility = Visibility.Visible;
        }

        public void HideTooltip()
        {
            // _tooltip.style.visibility = Visibility.Hidden;
        }

        private void RefreshInventoryUI()
        {
            _grid.Clear();

            foreach (var kvp in inventory) // kvp.Key = ItemKey, kvp.Value = quantity
            {
                var slotElement = slotTemplate.CloneTree();
                _grid.Add(slotElement);

                var slotUI = new SlotUI(slotElement, this);
                slotUI.SetData(kvp.Key, kvp.Value);
            }
        }
    }
}
