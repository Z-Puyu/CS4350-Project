using System;
using System.Collections.Generic;
using GameplayAbilities.Runtime.Attributes;
using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items;
using UnityEngine;

namespace Player {
    [DisallowMultipleComponent]
    public sealed class PlayerController : MonoBehaviour {
        [field: SerializeField] private PlayerData InitialData { get; set; }
        [field: SerializeField] private AttributeSet AttributeSet { get; set; }
        [field: SerializeField] private Inventory Inventory { get; set; }
        
        private void Start() {
            if (!this.InitialData) {
                return;
            }
            
            this.AttributeSet.Initialise(this.InitialData.Attributes);
            foreach (KeyValuePair<ItemData, int> data in this.InitialData.Items) {
                this.Inventory.Add(data.Value, Item.From(data.Key).Key);
            }
        }
    }
}
