using System.Collections.Generic;
using DataStructuresForUnity.Runtime.GeneralUtils;
using ModularItemsAndInventory.Runtime.Items;
using SaintsField;
using UnityEngine;

namespace ModularItemsAndInventory.Runtime.Inventory {
    public class ItemDatabase : Singleton<ItemDatabase> {
        [field: SerializeField, ResourceFolder] 
        public string ItemDataFolder { get; private set; }
        
        private Dictionary<string, ItemData> Items { get; } = new Dictionary<string, ItemData>();
        private Dictionary<ItemKey, Item> RuntimeItems { get; } = new Dictionary<ItemKey, Item>();

        protected override void Awake() {
            base.Awake();
            foreach (ItemData data in Resources.LoadAll<ItemData>(this.ItemDataFolder)) {
                if (this.Items.TryGetValue(data.Id, out ItemData existing)) {
                    Debug.LogError($"Duplicate item ID {data.Id} for {data.name} and {existing.name}", this);
                    return;
                }
                
                this.Items.Add(data.Id, data);
            }
            
            Debug.Log($"Loaded {this.Items.Count} items from {this.ItemDataFolder}", this);
        }

        public static bool IsRuntimeDefined(ItemKey key) {
            return Singleton<ItemDatabase>.Instance.Items.ContainsKey(key);
        }

        public static bool TryGet(string id, out ItemData data) {
            return Singleton<ItemDatabase>.Instance.Items.TryGetValue(id, out data);
        }

        public static bool TryGet(ItemKey key, out Item item) {
            ItemDatabase database = Singleton<ItemDatabase>.Instance;
            if (database.RuntimeItems.TryGetValue(key, out item)) {
                return true;
            }

            if (!database.Items.TryGetValue(key, out ItemData data)) {
                Debug.LogError($"Item {key} not found", database);
                return false;
            }

            item = Item.From(data);
            database.RuntimeItems.Add(key, item);
            return true;
        }

        public static bool Add(Item item) {
            return Singleton<ItemDatabase>.Instance.RuntimeItems.TryAdd(item.Key, item);
        }

        public static bool Remove(Item item) {
            return Singleton<ItemDatabase>.Instance.RuntimeItems.Remove(item.Key);
        }

        internal static ItemType TypeOf(ItemKey key) {
            return ItemDatabase.TryGet(key.Id, out ItemData data) ? data.Type : null;
        }
    }
}
