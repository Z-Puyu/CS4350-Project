using Farming_related;
using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items;
using ModularItemsAndInventory.Runtime.Items.Properties;
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
        private Button _sellButton;
        private Button _sellAllButton;
        private Button _shopInventoryButton;
        private Button _sellerInventoryButton;

        private bool shopOpen;

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
            _sellButton = _root.Q<Button>("SellButton");
            _sellAllButton = _root.Q<Button>("SellAllButton");
            _shopInventoryButton = _root.Q<Button>("ShopInventoryButton");
            _sellerInventoryButton = _root.Q<Button>("SellerInventoryButton");

            shopOpen = true;

            // Hide item description
            _itemDescriptionContainer.style.visibility = Visibility.Hidden;

            //Hide sell buttone first
            _sellButton.style.display = DisplayStyle.None;
            _sellAllButton.style.display = DisplayStyle.None;

            // Register button actions
            _buyButton.clicked += OnBuyButtonClicked;
            _sellButton.clicked += OnSellButtonClicked;
            _sellAllButton.clicked += OnSellAllButtonClicked;
            _shopInventoryButton.clicked += ChangeToShop;
            _sellerInventoryButton.clicked += ChangeToSell;


            // Listen to inventory changes
            playerInventory.OnInventoryChanged += HandleInventoryChanged;

            // Initial draw
            RefreshShopInventoryUI();
        }

        private void OnDisable()
        {
            playerInventory.OnInventoryChanged -= HandleInventoryChanged;
        }

        private void HandleInventoryChanged(Inventory.ItemOperation operation) //*********************NEED TO FIGURE OUT WHAT THIS DOES
        {
            if (shopOpen == true)
            {
                RefreshShopInventoryUI();
            }
            else
            {
                RefreshSellerInventoryUI();
            }
        }

        public void UpdateItemPanel(ItemKey itemKey, ItemData itemData)
        {
            _itemDescriptionContainer.style.visibility = Visibility.Visible;
            _currentItemKey = itemKey;
            _itemDescription.text = itemData.Description;
            _itemName.text = itemData.Name;
            _itemIcon.style.backgroundImage = new StyleBackground(itemData.Icon);
        }

        public void ClearItemPanel()
        {
            // Hide the whole description container
            _itemDescriptionContainer.style.visibility = Visibility.Hidden;

            // Clear stored item key
            _currentItemKey = null;

            // Clear the text fields
            _itemDescription.text = string.Empty;
            _itemName.text = string.Empty;

            // Optionally clear the icon too (if you set it dynamically elsewhere)
            _itemIcon.Clear();
        }

        private void RefreshShopInventoryUI()
        {
            _grid.Clear();
            Debug.Log($"CHECKPOINT 1");
            foreach (var kvp in shopInventory.ItemsForSale) // kvp.itemKey = ItemKey, kvp.stock = quantity
            {
                Debug.Log($"CHECKPOINT 2");
                if (ItemDatabase.TryGet(kvp.itemKey, out Item item))
                {
                    Debug.Log($"CHECKPOINT 3");
                    var slotElement = slotTemplate.CloneTree();
                    _grid.Add(slotElement);

                    var shopSlotUI = new ShopSlotUI(slotElement, this);
                    if (item.Properties.HaveExactly<Merchandise>(out Merchandise merchandise))
                    {
                        Debug.Log($"[SHOP INVENTORY] item name: {kvp.itemData.name}"); // Price when buying
                        Debug.Log($"[SHOP INVENTORY] Price: {merchandise.Price}"); // Price when buying
                        Debug.Log($"[SHOP INVENTORY] Worth: {merchandise.Worth}"); // Price when selling
                        shopSlotUI.SetData(kvp.itemKey, kvp.stock, merchandise.Price);
                    }
                    else
                    {
                        Debug.LogWarning($"Item {kvp.itemKey} has no price");
                    }


                }
                else
                {
                    Debug.LogWarning($"Item not found for key: {kvp.itemKey}");
                }

            }
        }

        private void RefreshSellerInventoryUI() //*************NEED TO FIX THIS********************
        {
            _grid.Clear();
            foreach (var kvp in playerInventory) // kvp.itemKey = ItemKey, kvp.stock = quantity
            {
                if (ItemDatabase.TryGet(kvp.Key, out Item item))
                {
                    var slotElement = slotTemplate.CloneTree();
                    _grid.Add(slotElement);
                    var shopSlotUI = new ShopSlotUI(slotElement, this);
                    if (item.Properties.HaveExactly<Merchandise>(out Merchandise merchandise))
                    {
                        Debug.Log($"[SELLER INVENTORY] item name: {item.Name}"); // Price when buying
                        Debug.Log($"[SELLER INVENTORY] Price: {merchandise.Price}"); // Price when buying
                        Debug.Log($"[SHSELLEROP INVENTORY] Worth: {merchandise.Worth}"); // Price when selling
                        shopSlotUI.SetData(kvp.Key, kvp.Value, merchandise.Worth);
                    }
                    else
                    {
                        Debug.LogWarning($"Item {kvp.Key} has no worth");
                    }
                }
                else
                {
                    Debug.LogWarning($"Item not found for key: {kvp.Key}");
                }
            }
        }
        public void SetShopInventory(ShopInventory inventory)
        {
            this.shopInventory = inventory;
            RefreshShopInventoryUI();
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
            RefreshShopInventoryUI();
        }

        private void ChangeToShop()
        {
            shopOpen = true;
            _buyButton.style.display = DisplayStyle.Flex;
            _sellButton.style.display = DisplayStyle.None;
            _sellAllButton.style.display = DisplayStyle.None;
            ClearItemPanel();
            RefreshShopInventoryUI();

        }

        private void ChangeToSell()
        {
            shopOpen = false;
            _buyButton.style.display = DisplayStyle.None;
            _sellButton.style.display = DisplayStyle.Flex;
            _sellAllButton.style.display = DisplayStyle.Flex;
            ClearItemPanel();
            RefreshSellerInventoryUI();
        }

        private void OnSellButtonClicked()
        {

        }

        private void OnSellAllButtonClicked()
        {

        }


        private void OnDropButtonClicked()
        {
            if (!_currentItemKey.HasValue) return;

            Debug.Log($"Drop item: {_currentItemKey}");
        }
    }

}
