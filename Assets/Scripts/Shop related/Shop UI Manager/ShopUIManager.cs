using Farming_related;
using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items;
using GameplayAbilities.Runtime.MoneySystem;
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
        [SerializeField] private Money PlayerMoney;

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
            _root = uiDocument.rootVisualElement;
            _grid = _root.Q<VisualElement>("Grid");

            _itemDescriptionContainer = _root.Q<VisualElement>("ItemDescriptionContainer");
            _itemName = _root.Q<Label>("ItemName");
            _itemDescription = _root.Q<Label>("ItemDescription");
            _itemIcon = _root.Q<VisualElement>("ItemIcon");

            _buyButton = _root.Q<Button>("BuyButton");

            // Hide item description
            _itemDescriptionContainer.style.visibility = Visibility.Hidden;

            // Register button actions
            _buyButton.clicked += OnBuyButtonClicked;

            // Listen to inventory changes
            playerInventory.OnInventoryChanged += HandleInventoryChanged;

            // Initial draw
            RefreshInventoryUI();
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
            _grid.Clear();
            foreach (var kvp in shopInventory.ItemsForSale) // kvp.itemKey = ItemKey, kvp.stock = quantity
            {

                var slotElement = slotTemplate.CloneTree();
                _grid.Add(slotElement);

                var shopSlotUI = new ShopSlotUI(slotElement, this);
                shopSlotUI.SetData(kvp.itemKey, kvp.stock);
            }
        }
        public void SetShopInventory(ShopInventory inventory)
        {
            this.shopInventory = inventory;
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

            // 2️⃣ Find the corresponding shop entry
            if (!shopInventory.TryGetItem(itemKey, out var shopItem))
            {
                Debug.LogWarning($"Shop does not have {itemData.Name} for sale.");
                return;
            }
            Debug.Log($"[MONEY] BEFORE Money current value is {PlayerMoney.Value}");
            PlayerMoney.Add(100); //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!Remember to delete this!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Debug.Log($"[MONEY] AFTER Money current value is {PlayerMoney.Value}");
            Debug.Log($"[MONEY] Item Data name: {itemData.Name}");
            Debug.Log($"[MONEY] shopItem.itemData.name: {shopItem.itemData.name}");
            Debug.Log($"[MONEY] shopItem price: {shopItem.price}");
            Debug.Log($"[MONEY] shopItem stock: {shopItem.stock}");
            // Debug.Log($"[MONEY] Item Data Price: {item.price}");
            // Debug.Log($"[MONEY] Item Data quantity: {item.stock}");

            if (!PlayerMoney.Spend(shopItem.price))
            {
                Debug.Log("Purchase failed — not enough money!");
                return;
            }
            else
            {
                Debug.Log($"[PURCHASE] Purchase made successfully! You just bought {shopItem.itemData.name}");
                Debug.Log($"[PURCHASE] AFTER Money current value is {PlayerMoney.Value}");
            }
            playerInventory.Add(itemKey);
            shopInventory.Remove(itemKey);
            RefreshInventoryUI();
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
