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
        [System.Serializable]
        public class ShopItem
        {
            public ItemData itemData;
            public int price;
            public int stock = -1;

            public ItemKey itemKey => ItemKey.From(itemData);
        }
        [Header("Shop Items")]
        [SerializeField] private List<ShopItem> itemsForSale = new List<ShopItem>();

        public IReadOnlyList<ShopItem> ItemsForSale => itemsForSale;

        public void Use(SaintsDictionary<ItemData, int> itemDataDict)
        {
            itemsForSale.Clear();

            foreach (var kvp in itemDataDict)
            {
                itemsForSale.Add(new ShopItem
                {
                    itemData = kvp.Key,
                    price = kvp.Value,
                    stock = -1 // Or whatever default stock
                });
            }
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

