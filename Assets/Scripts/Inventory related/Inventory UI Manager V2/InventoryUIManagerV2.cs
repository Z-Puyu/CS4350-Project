using System.Collections.Generic;
using Farming_related;
using Inventory_related.Inventory_UI_Manager;
using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Events;
using GameplayAbilities.Runtime.HealthSystem;

namespace Inventory_related.Inventory_UI_Manager_V2
{
    public class InventoryUIManagerV2 : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform gridContainer;           // GridContainer transform
        [SerializeField] private InventorySlot slotPrefab;          // Prefab reference
        [SerializeField] private ItemType seedType;                 // assign in inspector, your "Seed" type asset
        [SerializeField] private ItemType consumableType;
        [SerializeField] private CrossObjectEventSO unpausedEvent;
        [SerializeField] private Health playerHealth;

        [Header("Item Viewer UI")]
        [SerializeField] private GameObject itemViewerContainer;     // Entire right-hand panel
        [SerializeField] private Image itemImage;                    // ItemViewerContainer -> ItemImage
        [SerializeField] private TextMeshProUGUI itemNameText;       // ItemDetails -> ItemName
        [SerializeField] private TextMeshProUGUI itemDescriptionText;// ItemDetails -> ItemDescription

        [Header("Buttons")]
        [SerializeField] private Button useButton;
        [SerializeField] private Button quickSwapButton;
        [SerializeField] private Button dropButton;

        [Header("Other UI")]
        [SerializeField] private Button closeButton;

        [Header("Player Inventory")]
        [SerializeField] private Inventory inventory;

        private ItemData _currentItem;
        private ItemKey _currentItemKey;

        private Dictionary<string, InventorySlot> _mappedSlots = new();

        public static InventoryUIManagerV2 Instance { get; private set; }
        
        [SerializeField] private InventoryV2UIManagerSoilDetector soilDetector;
        [SerializeField] private CrossObjectEventSO broadcastPauseGame;

        private void Awake()
        {
            Instance = this;
            gameObject.SetActive(false);

            // Hide item description
            itemViewerContainer.SetActive(false);

            // Register button actions
            useButton.onClick.AddListener(OnUseItem);
            quickSwapButton.onClick.AddListener(OnQuickSwapItem);
            dropButton.onClick.AddListener(OnDropItem);
            closeButton.onClick.AddListener(CloseInventory);

            // Listen to inventory changes
            inventory.OnInventoryChanged += HandleInventoryChanged;

            // Initial draw
            RefreshInventoryUI();
        }

        private void HandleInventoryChanged(Inventory.ItemOperation operation)
        {
            RefreshInventoryUI();
        }

        private void RefreshInventoryUI()
        {
            foreach (Transform child in gridContainer)
                Destroy(child.gameObject);

            _mappedSlots.Clear();

            foreach (var kvp in inventory)
            {
                ItemKey itemKey = kvp.Key;
                int quantity = kvp.Value;

                if (!ItemDatabase.TryGet(itemKey, out Item item)) continue;
                if (!ItemDatabase.TryGet(item.Id, out ItemData itemData)) continue;

                var slot = Instantiate(slotPrefab, gridContainer);
                slot.Initialize(this);
                bool isInQuickSwap = inventory.isInQuickSwap(itemKey.Id);
                slot.SetData(itemData, itemKey, quantity, isInQuickSwap);

                _mappedSlots[itemKey.Id] = slot;
            }
        }

        public void OnSlotClicked(ItemData itemData, ItemKey itemKey)
        {
            Debug.Log("ON SLOT CLICKED");
            _currentItem = itemData;
            _currentItemKey = itemKey;

            itemViewerContainer.SetActive(true);

            itemImage.sprite = itemData.Icon;
            itemNameText.text = itemData.Name;
            itemDescriptionText.text = itemData.Description;

            quickSwapButton.GetComponentInChildren<TextMeshProUGUI>().text =
                inventory.isInQuickSwap(itemData.Id) ? "Quick Unequip" : "Quick Equip";
        }

        public void OnUseItem(ItemData itemData)
        {
            _currentItem = itemData;
            OnUseItem();
        }

        public void ClearItemPanel()
        {
            itemViewerContainer.SetActive(false);

            // Clear the text fields
            itemImage.sprite = null;
            itemNameText.text = string.Empty;
            itemDescriptionText.text = string.Empty;
        }
 
        private void OnUseItem()
        {
            if (_currentItem == null) return;
            SoilPlantInteraction CurrentSoil = soilDetector.GetCurrentSoil();

            Debug.Log($"[DEBUG] CurrentSoil is {(CurrentSoil == null ? "null" : "set")}");

            // 🌱 If it's a seed and we have soil selected, plant it
            if (CurrentSoil != null)
            {
                if (_currentItem.Type.BelongsTo(seedType))
                {
                    Debug.Log($"Planting seed: {_currentItem.Name}");
                    Debug.Log($"Animation to be played: {_currentItem.Id}_....");
                    CurrentSoil.PlantSeed(_currentItem.Id);

                    // Remove one item from inventory
                    inventory.Remove(_currentItemKey);

                    ClearItemPanel();
                    // Reset soil reference and close the inventory
                    CurrentSoil = null;
                }
                else if (_currentItem.Type.BelongsTo(consumableType))
                {
                    Debug.Log($"Using Consumable to plant: {_currentItem.Name}. Not allowed");
                    return;
                }
            }
            else if (_currentItem.Type.BelongsTo(consumableType))
            {
                Debug.Log($"Using Consumable: {_currentItem.Name}");
                playerHealth.Refill();
                inventory.Remove(_currentItemKey);
            }
            else
            {
                Debug.Log($"Use item: {_currentItem.Name} (not a seed, or no soil selected)");
            }

            unpausedEvent.TriggerEvent();
            gameObject.SetActive(false);
        }

        private void OnQuickSwapItem()
        {
            if (!_currentItem) return;
            Debug.Log($"Quick-swap item: {_currentItem.Name}");

            bool isInQuickSwap = inventory.HandleItemForQuickSwap(_currentItem.Id);
            _mappedSlots[_currentItemKey.Id].HandleItemForQuickSwap(isInQuickSwap);
        }

        private void OnDropItem()
        {
            if (!_currentItem) return;
            inventory.Remove(_currentItemKey);
            Debug.Log($"Dropped item: {_currentItem.Name}");
        }

        public void OpenForSeedSelection()
        {
            broadcastPauseGame.TriggerEvent();
            gameObject.SetActive(true);
            RefreshInventoryUI();
        }

        public void CloseInventory()
        {
            Debug.Log("ON SLOT CLICKED");
            // Play a subtle UI close sound or fade animation
            unpausedEvent.TriggerEvent();
            gameObject.SetActive(false);
        }
    }
}