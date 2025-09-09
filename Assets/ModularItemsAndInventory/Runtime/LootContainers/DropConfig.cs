using System;
using SaintsField;
using UnityEngine;

namespace ModularItemsAndInventory.Runtime.LootContainers {
    [Serializable]
    public struct DropConfig {
        [field: SerializeField] public int MaxCount { get; private set; }
        [field: SerializeField] public int Weight { get; private set; }
        [field: SerializeField, MinValue(1)] public int CountPerDrop { get; private set; }
    }
}
