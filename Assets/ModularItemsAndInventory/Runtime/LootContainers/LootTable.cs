using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModularItemsAndInventory.Runtime.Items;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace ModularItemsAndInventory.Runtime.LootContainers {
    [CreateAssetMenu(fileName = "New Loot Table", menuName = "Modular Items and Inventory/Loot Table")]
    public class LootTable : ScriptableObject, IEnumerable<KeyValuePair<ItemKey, DropConfig>> {
        [field: SerializeField, SaintsDictionary("Item", "Drop Config")]
        private SaintsDictionary<ItemData, DropConfig> Loots { get; set; }
        
        [field: SerializeField, SaintsDictionary("Item", "Count")]
        public SaintsDictionary<ItemData, int> AlwaysDrop { get; private set; }

        public bool IsEmpty => this.Loots.Count == 0 && this.AlwaysDrop.Count == 0;

        public IEnumerator<KeyValuePair<ItemKey, DropConfig>> GetEnumerator() {
            return this.Loots.Select(selector).GetEnumerator();

            KeyValuePair<ItemKey, DropConfig> selector(KeyValuePair<ItemData, DropConfig> pair) 
                => new KeyValuePair<ItemKey, DropConfig>(ItemKey.From(pair.Key), pair.Value);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
        
        [Button]
        private void LoadFromAnotherTable(LootTable table, bool overwrite) {
            foreach ((ItemData item, DropConfig config) in table.Loots) {
                if (this.Loots.ContainsKey(item) && !overwrite) {
                    continue;
                } 
                
                this.Loots[item] = config;
            }
            
            foreach ((ItemData item, int count) in table.AlwaysDrop) {
                if (this.AlwaysDrop.ContainsKey(item) && !overwrite) {
                    continue;
                }
                
                this.AlwaysDrop[item] = count;
            }
        }
    }
}