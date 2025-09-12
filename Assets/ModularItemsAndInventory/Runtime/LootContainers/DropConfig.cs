using System;
using SaintsField;
using UnityEngine;

namespace ModularItemsAndInventory.Runtime.LootContainers {
    [Serializable]
    public struct DropConfig {
        [field: SerializeField] public int MaxDrop { get; private set; }
        [field: SerializeField] public int Weight { get; private set; }

        [field: SerializeField, MinMaxSlider(1, 10)] 
        private Vector2Int CountPerDrop { get; set; }

        public int DropCount => this.CountPerDrop.x == this.CountPerDrop.y
                ? this.CountPerDrop.x
                : UnityEngine.Random.Range(this.CountPerDrop.x, this.CountPerDrop.y + 1);
    }
}
