using System.Collections.Generic;
using System.Linq;
using ModularItemsAndInventory.Runtime.Items;
using SaintsField;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ModularItemsAndInventory.Runtime.LootContainers {
    [DisallowMultipleComponent]
    public class LootContainer : MonoBehaviour {
        [field: SerializeField] private LootTable LootTable { get; set; }
        
        [field: SerializeField, MinMaxSlider(1, 20)] 
        private Vector2Int RandomDropAmount { get; set; } = new Vector2Int(1, 5);

        private Dictionary<ItemKey, DropConfig> Loots { get; set; } =
            new Dictionary<ItemKey, DropConfig>();

        private Dictionary<ItemKey, int> Container { get; set; } = new Dictionary<ItemKey, int>();
        private bool HasBeenOpenedBefore { get; set; }

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
        public void ShouldDrop(ItemKey item, int count) {
            this.Container.Add(item, count);
        }

        /// <summary>
        /// Injects a drop configuration into the loot container.
        /// </summary>
        /// <param name="item">The item to drop.</param>
        /// <param name="config">The drop configuration.</param>
        public void ShouldRandomlyDrop(ItemKey item, DropConfig config) {
            this.Loots[item] = config;
        }

        private float ComputeTotalWeight() {
            return this.Loots.Values.Sum(config => config.Weight);
        }

        private void DropRandomly() {
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

                    this.Container[item] = this.Container.GetValueOrDefault(item, 0) + 
                                           config.CountPerDrop;
                    if (this.Container[item] >= config.MaxCount) {
                        this.Loots.Remove(item);
                    }
                    
                    break;
                }
            }
        
            this.HasBeenOpenedBefore = true;
        }
    }
}
