using UnityEngine;
using ModularItemsAndInventory.Runtime.Items;

namespace Shop.Runtime
{
    [System.Serializable]
    public class ShopItemData
    {
        public ItemData itemData;
        public int price;
        public int stock = -1;

        public ItemKey itemKey => ItemKey.From(itemData);
    }
}

