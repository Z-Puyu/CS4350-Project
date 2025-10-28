using GameplayAbilities.Runtime.Attributes;
using ModularItemsAndInventory.Runtime.Items;
using SaintsField;
using System.Collections.Generic;
using UnityEngine;
using Shop.Runtime;

namespace Game.NPC
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New NPC Data", menuName = "NPC/Shopkeeper Data")]
    public class ShopKeeperData : ScriptableObject
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public AttributeTable Attributes { get; private set; }
        [field: SerializeField] public AnimatorOverrideController Animations { get; private set; }
        [field: SerializeField] public SaintsDictionary<ItemData, int> Items { get; private set; }
        [Header("Shop Inventory")]
        [SerializeField] public List<ShopItemData> itemsForSale = new();
        public IReadOnlyList<ShopItemData> ItemsForSale => itemsForSale;
    }

}
