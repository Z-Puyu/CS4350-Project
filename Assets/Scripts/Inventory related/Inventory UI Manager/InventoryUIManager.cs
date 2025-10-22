using System.Collections.Generic;
using Farming_related;
using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items;
using UnityEngine;
using UnityEngine.UIElements;

namespace Inventory_related.Inventory_UI_Manager
{
    public class InventoryUIManager : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private VisualTreeAsset slotTemplate; // assign Slot.uxml in inspector
        [SerializeField] private Inventory inventory;
        [SerializeField] private ItemType seedType; // assign in inspector, your "Seed" type asset

        private ItemKey? _currentItemKey;
        
        private VisualElement _root;
        private VisualElement _grid;

        private VisualElement _itemDescriptionContainer;
        private Label _itemDescription;
        private Label _itemName;
        private VisualElement _itemIcon;

        public static InventoryUIManager Instance;

        public SoilPlantInteraction CurrentSoil { get; private set; }
        
        // Buttons
        private Button _useButton;
        private Button _dropButton;
        private Button _quickEquipButton;

        private Dictionary<string, SlotUI> mappedIdToSlotUI = new Dictionary<string, SlotUI>();

        private void Awake()
        {
            Instance = this;
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _root = uiDocument.rootVisualElement;
            _grid = _root.Q<VisualElement>("Grid");

            _itemDescriptionContainer = _root.Q<VisualElement>("ItemDescriptionContainer");
            _itemName = _root.Q<Label>("ItemName");
            _itemDescription = _root.Q<Label>("ItemDescription");
            _itemIcon = _root.Q<VisualElement>("ItemIcon");
            
            _useButton = _root.Q<Button>("UseButton");
            _dropButton = _root.Q<Button>("DropButton");
            _quickEquipButton =  _root.Q<Button>("QuickEquipButton");

            // Hide item description
            _itemDescriptionContainer.style.visibility = Visibility.Hidden;
            
            // Register button actions
            _useButton.clicked += OnUseButtonClicked;
            _dropButton.clicked += OnDropButtonClicked;
            _quickEquipButton.clicked += OnQuickEquipButtonClicked;

            // Listen to inventory changes
            inventory.OnInventoryChanged += HandleInventoryChanged;

            // Initial draw
            RefreshInventoryUI();
        }

        private void OnDisable()
        {
            mappedIdToSlotUI.Clear();
            inventory.OnInventoryChanged -= HandleInventoryChanged;
        }

        private void HandleInventoryChanged(Inventory.ItemOperation operation)
        {
            RefreshInventoryUI();
        }

        public void UpdateItemPanel(ItemKey itemKey, ItemData itemData)
        {
            _itemDescriptionContainer.style.visibility = Visibility.Visible;
            _currentItemKey = itemKey;
            _itemDescription.text = itemData.Description;
            _itemName.text = itemData.Name;
            if (inventory.isInQuickSwap(itemData.Id))
            {
                _quickEquipButton.text = "Quick unequip";
            }
            else
            {
                _quickEquipButton.text = "Quick equip";
            }
        }

        private void RefreshInventoryUI()
        {
            _grid.Clear();
            
            foreach (var kvp in inventory) // kvp.Key = ItemKey, kvp.Value = quantity
            {
                var slotElement = slotTemplate.CloneTree();
                _grid.Add(slotElement);
                
                var slotUI = new SlotUI(slotElement, this, inventory.isInQuickSwap(kvp.Key.Id));
                mappedIdToSlotUI[kvp.Key.Id] = slotUI;
                slotUI.SetData(kvp.Key, kvp.Value);
            }
        }
        
        // === Button handlers ===
        private void OnUseButtonClicked()
        {
            if (!_currentItemKey.HasValue) return;

            var itemKey = _currentItemKey.Value;

            // Look up the item
            if (!ItemDatabase.TryGet(itemKey, out Item item))
            {
                Debug.LogWarning($"Item {itemKey} not found in database.");
                return;
            }

            var itemData = item;

            // 🌱 If it's a seed and we have soil selected, plant it
            Debug.Log($"[DEBUG] CurrentSoil is {(CurrentSoil == null ? "null" : "set")}");
            if (itemData.Type.BelongsTo(seedType) && CurrentSoil != null)
            {
                CurrentSoil.PlantSeed(itemData.Id);
                inventory.Remove(itemKey); // remove 1 seed
                CurrentSoil = null;
                gameObject.SetActive(false);
            }
            else
            {
                Debug.Log($"[DEBUG] Item '{itemData.Name}' has type '{itemData.Type?.name}'");
                Debug.Log($"[DEBUG] SeedType reference is '{seedType?.name}'");
                Debug.Log($"[DEBUG] BelongsTo result: {itemData.Type?.BelongsTo(seedType)}");
                Debug.Log($"Use item: {itemData.Name} (not a seed, or no soil selected)");
            }
        }
        
        private void OnDropButtonClicked()
        {
            if (!_currentItemKey.HasValue) return;

            Debug.Log($"Drop item: {_currentItemKey}");
        }

        private void OnQuickEquipButtonClicked()
        {
            if (!_currentItemKey.HasValue) return;
            
            bool isInQuickSwap = inventory.HandleItemForQuickSwap(_currentItemKey.Value.Id);
            mappedIdToSlotUI[_currentItemKey.Value.Id].HandleItemForQuickSwap(isInQuickSwap);
        }

        public void OpenForSeedSelection(SoilPlantInteraction soil)
        {
            CurrentSoil = soil;
            gameObject.SetActive(true);
            RefreshInventoryUI();
        }
    }
}
