using Farming_related;
using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items;
using ModularItemsAndInventory.Runtime.Items.Properties;
using GameplayAbilities.Runtime.MoneySystem;
using WeaponsSystem.Runtime.WeaponComponents;
using Shop.Runtime;
using UnityEngine;
using UnityEngine.UIElements;
using Game.NPC;
using System.Diagnostics;
using System.Collections.Generic;
using Events;

namespace Shop_related.Shop_UI_Manager
{
    public class BlacksmithUIManager : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private VisualTreeAsset slotTemplate; // assign Slot.uxml in inspector
        [SerializeField] private Inventory playerInventory;
        [SerializeField] private BlacksmithInventory blacksmithInventory;
        [SerializeField] private Money PlayerMoney;

        [SerializeField] private ItemType seedType; // assign in inspector, your "Seed" type asset
        [SerializeField] private CrossObjectEventSO broadcastPauseGame;
        [SerializeField] private CrossObjectEventSO broadcastUnPauseGame;

        private ItemKey? _currentItemKey;
        private WeaponComponent _currentComponent;

        private VisualElement _root;
        private VisualElement _grid;

        private VisualElement _itemDescriptionContainer;
        private VisualElement _materialCostContainer;
        private Label _materialCost;
        private Label _itemName;

        private Label _moneyCount;

        private VisualElement _itemMainIcon;

        public static BlacksmithUIManager Instance;

        public SoilPlantInteraction CurrentSoil { get; private set; }

        // Buttons
        private Button _buyButton;
        private Button _equipButton;
        private Button _meleeButton;
        private Button _rangedButton;
        private Button _placableButton;
        private Button _exitButton;
        

        private string currentWeaponCategory = "Melee";
        private List<(ItemKey material, int amount)> requiredMaterials = new List<(ItemKey, int)>();
        private string currentComponentName;

        private void Awake()
        {
            Instance = this;
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            broadcastPauseGame.TriggerEvent();
            _root = uiDocument.rootVisualElement;
            _grid = _root.Q<VisualElement>("Grid");

            _itemDescriptionContainer = _root.Q<VisualElement>("ItemDescriptionContainer");
            _materialCostContainer = _root.Q<VisualElement>("MaterialCostContainer");
            _itemName = _root.Q<Label>("ItemName");
            _moneyCount = _root.Q<Label>("MoneyCount");
            _materialCost = _root.Q<Label>("MaterialCost");
            _itemMainIcon = _root.Q<VisualElement>("ItemMainIcon");

            _buyButton = _root.Q<Button>("BuyButton");
            _equipButton = _root.Q<Button>("EquipButton");
            _meleeButton = _root.Q<Button>("MeleeButton");
            _rangedButton = _root.Q<Button>("RangedButton");
            _placableButton = _root.Q<Button>("PlaceableButton");
            _exitButton = _root.Q<Button>("ExitButton");


            // Hide item description
            _itemDescriptionContainer.style.visibility = Visibility.Hidden;


            _equipButton.style.display = DisplayStyle.None;

            // Register button actions
            _buyButton.clicked += OnBuyButtonClicked;
            // _equipButton.clicked += ;
            _meleeButton.clicked += ChangeToMelee;
            _rangedButton.clicked += ChangeToRanged;
            _placableButton.clicked += ChangeToPlaceable;
            _exitButton.clicked += ExitShop;


            // Listen to inventory changes
            playerInventory.OnInventoryChanged += HandleInventoryChanged;

            // Initial draw
            RefreshInventoryUI();
        }

        private void OnDisable()
        {
            broadcastUnPauseGame.TriggerEvent();
            playerInventory.OnInventoryChanged -= HandleInventoryChanged;
        }

        private void ExitShop()
        {
            gameObject.SetActive(false);
            ClearItemPanel();
            _grid.Clear();

            UnityEngine.Debug.Log("[BLACKSMITH UI] Closed shop interface.");
            var blacksmith = FindAnyObjectByType<Blacksmith>();
            if (blacksmith != null)
            {
                blacksmith.ResetInteraction();
            }
            UnityEngine.Debug.Log("[BLACKSMITH UI] Closed shop interface and reset interaction.");
        }

        private void HandleInventoryChanged(Inventory.ItemOperation operation)
        {
            RefreshInventoryUI();
        }

        private int GetPlayerAmount(ItemData material)
        {
            if (material == null || playerInventory == null)
                return 0;

            // Convert ItemData to ItemKey using its Id
            ItemKey key = ItemKey.FromID(material.Id);

            // Query the player's inventory for quantity of this material
            return playerInventory.Count(key);
        }

        public void UpdateItemPanel(ItemKey itemKey, WeaponComponent component)
        {
            _itemDescriptionContainer.style.visibility = Visibility.Visible;
            _currentItemKey = itemKey;
            _currentComponent = component;
            _itemName.text = component.ItemName;
            _itemMainIcon.style.backgroundImage = new StyleBackground(component.icon);
            currentComponentName = component.name;
            ItemKey itemKey2 = component.GetItemKey();
            if (playerInventory.HasComponent(currentComponentName))
            {
                _materialCostContainer.style.display = DisplayStyle.None;
                _buyButton.style.display = DisplayStyle.None;
                _equipButton.style.display = DisplayStyle.Flex;
            }
            else
            {
                _materialCostContainer.style.display = DisplayStyle.Flex;
                _buyButton.style.display = DisplayStyle.Flex;
                _equipButton.style.display = DisplayStyle.None;
                _materialCostContainer.Clear();
                requiredMaterials.Clear();
                if (component.craftingMaterials != null && component.craftingMaterials.Count > 0)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    foreach (var matCost in component.craftingMaterials)
                    {
                        if (matCost.material != null)
                        {
                            // Get how many the player has
                            int playerAmount = GetPlayerAmount(matCost.material);

                            // Create a label for this material
                            Label materialLabel = new Label($"{matCost.material.name} x{matCost.amount}");
                            materialLabel.AddToClassList("material-label");

                            // Set color based on whether player has enough
                            if (playerAmount >= matCost.amount)
                            {
                                materialLabel.style.color = new StyleColor(Color.green);
                            }
                            else
                            {
                                materialLabel.style.color = new StyleColor(Color.red);
                            }

                            _materialCostContainer.Add(materialLabel);
                            requiredMaterials.Add((ItemKey.FromID(matCost.material.Id), matCost.amount));
                        }
                    }
                }
                else
                {
                    Label noMaterialsLabel = new Label("No crafting materials");
                    _materialCostContainer.Add(noMaterialsLabel);
                }
            }

        }

        public void ClearItemPanel()
        {
            // Hide the whole description container
            _itemDescriptionContainer.style.visibility = Visibility.Hidden;

            // Clear stored item key
            _currentItemKey = null;

            // Clear the text fields
            _materialCostContainer.Clear();
            requiredMaterials.Clear();
            _itemName.text = string.Empty;

            _buyButton.style.display = DisplayStyle.Flex;
            _equipButton.style.display = DisplayStyle.None;

            // Optionally clear the icon too (if you set it dynamically elsewhere)
            _itemMainIcon.Clear();
        }

        private void RefreshInventoryUI()
        {
            _grid.Clear();
            _moneyCount.text = "$" + PlayerMoney.Value.ToString();
            foreach (var kvp in blacksmithInventory.WeaponsForSale) // kvp.itemKey = ItemKey, kvp.stock = quantity
            {

                if (ComponentDatabase.TryGet(kvp.name, out WeaponComponent component) && string.Equals(component.weaponCategory.ToString(), currentWeaponCategory, System.StringComparison.OrdinalIgnoreCase))
                {
                    var slotElement = slotTemplate.CloneTree();
                    _grid.Add(slotElement);

                    var blacksmithSlotUI = new BlacksmithSlotUI(slotElement, this);
                    blacksmithSlotUI.SetData(kvp.name);
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"Item not found for key: {kvp.name}");
                }

            }
        }
        public void SetBlacksmithInventory(BlacksmithInventory inventory)
        {
            this.blacksmithInventory = inventory;
            RefreshInventoryUI();
        }

        // === Button handlers ===
        private void OnBuyButtonClicked()
        {
            if (!_currentItemKey.HasValue) return;

            var itemKey = _currentItemKey.Value;
            // Look up the item (Error, might just remove it since It's only gonna have components)
            // if (!ComponentDatabase.TryGet(itemKey, out WeaponComponent component))
            // {
            //     UnityEngine.Debug.LogWarning($"Item {itemKey} not found in database.");
            //     return;
            // }

            // 2. Check if player has enough materials
            foreach (var material in requiredMaterials)
            {
                ItemKey materialKey = material.material;
                int amountNeeded = material.amount;

                int playerAmount = playerInventory.Count(materialKey);
                if (playerAmount < amountNeeded)
                {
                    if (ItemDatabase.TryGet(materialKey, out Item materialItem))
                    {
                        UnityEngine.Debug.Log($"[MATERIALS] Not enough {materialItem.Name}. Required: {amountNeeded}, You have: {playerAmount}");
                    }
                    else
                    {
                        UnityEngine.Debug.Log($"[MATERIALS] Not enough material {materialKey}. Required: {amountNeeded}, You have: {playerAmount}");
                    }
                    return; // Early exit, player lacks materials
                }
            }

            // 3. Remove required materials from inventory
            foreach (var material in requiredMaterials)
            {
                playerInventory.Remove(material.amount, material.material);
            }

            // 4. Add the purchased item to player's inventory
            if (!playerInventory.HasComponent(currentComponentName))
            {
                playerInventory.AddComponent(currentComponentName);
                UnityEngine.Debug.Log($"[Materials PURCHASE] Added component {currentComponentName} to player inventory.");
            }
            // playerInventory.Add(itemKey);

            // 5. Remove from blacksmith's stock
            // blacksmithInventory.Remove(itemKey);

            UnityEngine.Debug.Log($"[PURCHASE] Successfully purchased {itemKey}.");
            UpdateItemPanel(_currentItemKey.Value, _currentComponent);

            // 6. Refresh UI
            RefreshInventoryUI();
        }

        private void ChangeToMelee()
        {
            currentWeaponCategory = "Melee";
            UnityEngine.Debug.Log($"[BLACKSMITH] ChangeToMelee category: {currentWeaponCategory}");
            ClearItemPanel();
            RefreshInventoryUI();
        }
        private void ChangeToRanged()
        {
            currentWeaponCategory = "Ranged";
            UnityEngine.Debug.Log($"[BLACKSMITH] ChangeToRanged category: {currentWeaponCategory}");
            ClearItemPanel();
            RefreshInventoryUI();
        }
        private void ChangeToPlaceable()
        {
            currentWeaponCategory = "Placeable";
            UnityEngine.Debug.Log($"[BLACKSMITH] ChangeToPlaceable category: {currentWeaponCategory}");
            ClearItemPanel();
            RefreshInventoryUI();
        }
    }

}
