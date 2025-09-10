using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModularItemsAndInventory.Runtime.Items;
using SaintsField;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ModularItemsAndInventory.Runtime.LootContainers {
    [DisallowMultipleComponent]
    public class LootContainer : MonoBehaviour, IEnumerable<KeyValuePair<ItemKey, int>> {
        [field: SerializeField, Required] private LootTable LootTable { get; set; }
        
        [field: SerializeField, MinMaxSlider(1, 20)] 
        private Vector2Int RandomDropAmount { get; set; } = new Vector2Int(1, 5);

        private Dictionary<ItemKey, DropConfig> Loots { get; set; } =
            new Dictionary<ItemKey, DropConfig>();

        private Dictionary<ItemKey, int> Container { get; set; } = new Dictionary<ItemKey, int>();
        private bool HasBeenOpenedBefore { get; set; }
        public bool IsEmpty => this.Container.Count == 0;

        private void Start() {
            if (!this.LootTable) {
                return;
            }
            
            foreach ((ItemKey item, DropConfig config) in this.LootTable) {
                this.Loots.Add(item, config);
            }
            
            foreach ((ItemData item, int count) in this.LootTable.AlwaysDrop) {
                this.Container.Add(ItemKey.From(item), count);
            }
        }

        /// <summary>
        /// Injects an item that must be dropped into the container.
        /// </summary>
        /// <param name="item">The item to drop.</param>
        /// <param name="count">The count of the item to drop.</param>
        /// <returns>The loot container instance.</returns>
        public LootContainer ShouldDrop(ItemKey item, int count) {
            this.Container.Add(item, count);
            return this;
        }

        /// <summary>
        /// Injects a drop configuration into the loot container.
        /// </summary>
        /// <param name="item">The item to drop.</param>
        /// <param name="config">The drop configuration.</param>
        /// <returns>The loot container instance.</returns>
        public LootContainer ShouldRandomlyDrop(ItemKey item, DropConfig config) {
            this.Loots[item] = config;
            return this;
        }

        private float ComputeTotalWeight() {
            return this.Loots.Values.Sum(config => config.Weight);
        }

        public void Open() {
            if (this.HasBeenOpenedBefore) {
                return;
            }
        
            int count = Random.Range(this.RandomDropAmount.x, this.RandomDropAmount.y + 1);
            float total = this.ComputeTotalWeight();
            for (int i = 0; i < count; i += 1) {
                float select = Random.Range(0, total);
                float current = 0;
                foreach ((ItemKey item, DropConfig config) in this.Loots) {
                    current += config.Weight;
                    if (select >= current) {
                        continue;
                    }

                    this.Container[item] = this.Container.GetValueOrDefault(item, 0) + config.DropCount;
                    if (this.Container[item] >= config.MaxDrop) {
                        this.Loots.Remove(item);
                    }
                    
                    break;
                }
            }
        
            this.HasBeenOpenedBefore = true;
        }

        public IEnumerator<KeyValuePair<ItemKey, int>> GetEnumerator() {
            return this.Container.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
