using Events;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Shop.Runtime;
using UnityEngine;
using Shop_related.Shop_UI_Manager;
using ModularItemsAndInventory.Runtime.Items;

namespace Game.NPC
{
    public class ShopKeeper : MonoBehaviour
    {
        [Header("Shopkeeper Setup")]
        [field: SerializeField] private ShopKeeperData Data { get; set; }
        [field: SerializeField] private Animator Animator { get; set; }
        [Header("Shop Inventory")]
        [field: SerializeField] private ShopInventory Inventory { get; set; }

        [Header("Shop UI Event")]
        [SerializeField] private CrossObjectEventWithDataSO broadcastOpenShopUI;
        [SerializeField] private ShopUIManager shopUIManager;

        private bool hasInteracted = false;

        protected void Start()
        {
            if (!this.Data)
            {
                return;
            }

            if (this.Animator)
            {
                this.Animator.runtimeAnimatorController = this.Data.Animations;
            }
            // Convert SaintsDictionary<ItemData, int> → List<ShopItemData>
            var shopItems = new List<ShopItemData>();

            foreach (var kvp in this.Data.itemsForSale)
            {
                ItemData itemData = kvp.Key;
                int stock = kvp.Value;

                // Build the ShopItemData object
                var shopItem = new ShopItemData
                {
                    itemData = itemData,
                    price = 0,
                    stock = stock
                };

                shopItems.Add(shopItem);
            }

            this.Inventory.Use(shopItems);
            // this.Inventory.Use(this.Data.itemsForSale);
        }
        public void Interact()
        {

            if (hasInteracted)
                return;

            hasInteracted = true;

            if (!Inventory)
            {
                Debug.LogWarning("[ShopKeeper] No Inventory assigned.", this);
                return;
            }

            OpenShopUI();
        }

        public void ResetInteraction()
        {
            hasInteracted = false;
        }
        public void OpenShopUI()
        {
            if (shopUIManager == null)
            {
                Debug.LogWarning("[ShopKeeper] No ShopUIManager assigned!", this);
                return;
            }

            shopUIManager.gameObject.SetActive(true);
            shopUIManager.SetShopInventory(this.Inventory);

        }
    }

}
