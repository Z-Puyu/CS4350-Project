using System;
using System.Collections.Generic;
using Common;
using GameplayAbilities.Runtime.Attributes;
using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items;
using UnityEngine;

namespace Player {
    [DisallowMultipleComponent]
    public sealed class PlayerController : MonoBehaviour, ICollector {
        [field: SerializeField] private PlayerData InitialData { get; set; }
        [field: SerializeField] private AttributeSet AttributeSet { get; set; }
        [field: SerializeField] private Inventory Inventory { get; set; }
        
        private void Start() {
            if (!this.InitialData) {
                return;
            }
            
            this.AttributeSet.Initialise(this.InitialData.Attributes);
            foreach (KeyValuePair<ItemData, int> data in this.InitialData.Items) {
                this.Inventory.Add(data.Value, ItemKey.From(data.Key));
            }
        }

        public void Collect(int count, ItemKey item) {
            this.Inventory.Add(count, item);
            OnScreenDebugger.Log($"Collected {count} {item.Id}");
            OnScreenDebugger.Log("Current Inventory:");
            foreach (KeyValuePair<ItemKey, int> pair in this.Inventory) {
                OnScreenDebugger.Log($"{pair.Key.Id}: {pair.Value}");
            }
        }
    }
}
