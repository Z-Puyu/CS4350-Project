using System;
using System.Collections.Generic;
using SaintsField;
using ModularItemsAndInventory.Runtime.Items;
using UnityEngine;

namespace Shop.Runtime
{
    /// <summary>
    /// A runtime shop inventory that stores which items the shop sells.
    /// Attach this script to an empty GameObject in your scene or prefab.
    /// </summary>
    public class ShopInventory : MonoBehaviour
    {
        // [System.Serializable]
        public bool TryGetItem(ItemKey key, out ShopItemData foundItem)
        {
            foundItem = itemsForSale.Find(i => i.itemKey.Equals(key));
            return foundItem != null;
        }
        [Header("Shop Items")]
        [SerializeField] private List<ShopItemData> itemsForSale = new List<ShopItemData>();

        public IReadOnlyList<ShopItemData> ItemsForSale => itemsForSale;

        public void Use(List<ShopItemData> shopItems)
        {
            itemsForSale.Clear();
            foreach (var saleItems in shopItems)
            {
                // itemsForSale.Add(new ShopItemData
                // {
                //     itemData = saleItems.itemData,
                //     price = saleItems.price,
                //     stock = saleItems.stock // Or whatever default stock
                // });
                itemsForSale.Add(saleItems);
            }
        }

        public bool Remove(ItemKey item)
        {
            return Remove(1, item);
        }

        public bool Remove(int quantity, ItemKey item)
        {
            if (quantity < 1)
            {
                Debug.LogWarning("You must remove at least one unit of an item.", this);
                return false;
            }

            // Find the item in the shop list
            var shopItem = itemsForSale.Find(i => i.itemKey.Equals(item));
            if (shopItem == null)
            {
                Debug.LogWarning($"Shop does not have any {item} to remove.", this);
                return false;
            }

            // Infinite stock (-1) → cannot remove from count, but you might allow complete removal
            if (shopItem.stock == -1)
            {
                Debug.Log($"Item {item} has infinite stock. Removing listing instead.", this);
                itemsForSale.Remove(shopItem);
                return true;
            }

            // Decrease stock
            if (shopItem.stock < quantity)
            {
                Debug.LogWarning($"Trying to remove {quantity} of {item} but only {shopItem.stock} left.", this);
                quantity = shopItem.stock;
            }

            shopItem.stock -= quantity;

            // If stock is depleted, remove from list
            if (shopItem.stock <= 0)
            {
                itemsForSale.Remove(shopItem);
                Debug.Log($"Removed {item} from shop (stock depleted).", this);
            }

            return true;
        }

        public bool HasItem(ItemKey key) =>
            itemsForSale.Exists(i => i.itemKey.Equals(key));

        public int GetPrice(ItemKey key)
        {
            var item = itemsForSale.Find(i => i.itemKey.Equals(key));
            return item != null ? item.price : -1;
        }

        public bool TryPurchase(ItemKey key, int quantity, out int totalCost)
        {
            var item = itemsForSale.Find(i => i.itemKey.Equals(key));
            if (item == null)
            {
                totalCost = 0;
                return false;
            }

            totalCost = item.price * quantity;
            if (item.stock == 0)
                return false;

            if (item.stock > 0)
                item.stock -= quantity;

            return true;
        }
    }
}

