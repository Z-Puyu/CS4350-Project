using Farming_related;
using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items;
using Shop.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace Shop_related.Shop_UI_Manager
{
    public class ShopUIManager : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private VisualTreeAsset slotTemplate; // assign Slot.uxml in inspector
        [SerializeField] private Inventory playerInventory;
        [SerializeField] private ShopInventory shopInventory;
        [SerializeField] private ItemType seedType; // assign in inspector, your "Seed" type asset

        private ItemKey? _currentItemKey;

        private VisualElement _root;
        private VisualElement _grid;

        private VisualElement _itemDescriptionContainer;
        private Label _itemDescription;
        private Label _itemName;
        private VisualElement _itemIcon;

        public static ShopUIManager Instance;

        public SoilPlantInteraction CurrentSoil { get; private set; }

        // Buttons
        private Button _buyButton;

        private void Awake()
        {
            Instance = this;
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            Debug.Log("[ShopKeeper] Checkpoint 1", this);
            _root = uiDocument.rootVisualElement;
            _grid = _root.Q<VisualElement>("Grid");
            Debug.Log("[ShopKeeper] Checkpoint 2", this);

            _itemDescriptionContainer = _root.Q<VisualElement>("ItemDescriptionContainer");
            _itemName = _root.Q<Label>("ItemName");
            _itemDescription = _root.Q<Label>("ItemDescription");
            _itemIcon = _root.Q<VisualElement>("ItemIcon");
            Debug.Log("[ShopKeeper] Checkpoint 3", this);

            _buyButton = _root.Q<Button>("BuyButton");
            Debug.Log("[ShopKeeper] Checkpoint 4", this);

            // Hide item description
            _itemDescriptionContainer.style.visibility = Visibility.Hidden;
            Debug.Log("[ShopKeeper] Checkpoint 5", this);

            // Register button actions
            _buyButton.clicked += OnBuyButtonClicked;
            Debug.Log("[ShopKeeper] Checkpoint 6", this);

            // Listen to inventory changes
            playerInventory.OnInventoryChanged += HandleInventoryChanged;
            Debug.Log("[ShopKeeper] Checkpoint 7", this);

            // Initial draw
            RefreshInventoryUI();
            Debug.Log("[ShopKeeper] Checkpoint 8", this);
        }

        private void OnDisable()
        {
            playerInventory.OnInventoryChanged -= HandleInventoryChanged;
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
        }

        private void RefreshInventoryUI()
        {
            Debug.Log("[ShopKeeper] Refresh Inventory UI start", this);
            Debug.Log($"[ShopKeeper] _grid: {_grid}, is null: {_grid == null}", this);
            _grid.Clear();

            Debug.Log("[ShopKeeper] for each start start", this);
            foreach (var kvp in shopInventory.ItemsForSale) // kvp.Key = ItemKey, kvp.Value = quantity
            {
                var slotElement = slotTemplate.CloneTree();
                _grid.Add(slotElement);

                var shopSlotUI = new ShopSlotUI(slotElement, this);
                shopSlotUI.SetData(kvp.itemKey, kvp.stock);
            }
            Debug.Log("[ShopKeeper] for each start end", this);
        }
        public void SetShopInventory(ShopInventory inventory)
        {
            Debug.Log("[ShopKeeper] Set Shop Inventory called start", this);
            this.shopInventory = inventory;
            Debug.Log("[ShopKeeper] Set Shop Inventory called End", this);
            Debug.Log($"[ShopKeeper] shopInventory: {shopInventory}", this);
            RefreshInventoryUI();
        }

        // === Button handlers ===
        private void OnBuyButtonClicked()
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
                playerInventory.Remove(itemKey); // remove 1 seed
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

        public void OpenForSeedSelection(SoilPlantInteraction soil)
        {
            CurrentSoil = soil;
            gameObject.SetActive(true);
            RefreshInventoryUI();
        }
    }

}
