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

        private VisualElement _grid;
        
        private void OnEnable()
        {
            var root = uiDocument.rootVisualElement;
            _grid = root.Q<VisualElement>("InventoryGrid");

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

        private void RefreshInventoryUI()
        {
            _grid.Clear();

            foreach (var kvp in inventory) // kvp.Key = ItemKey, kvp.Value = quantity
            {
                var slotElement = slotTemplate.CloneTree();
                _grid.Add(slotElement);

                var slotUI = new SlotUI(slotElement);
                slotUI.SetData(kvp.Key, kvp.Value);
            }
        }
    }
}
