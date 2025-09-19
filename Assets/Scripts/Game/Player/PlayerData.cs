using GameplayAbilities.Runtime.Attributes;
using ModularItemsAndInventory.Runtime.Items;
using SaintsField;
using UnityEngine;

namespace Game.Player {
    [CreateAssetMenu(fileName = "New Player Data", menuName = "Player/Player Data")]
    public sealed class PlayerData : ScriptableObject {
        [field: SerializeField] public AttributeTable Attributes { get; private set; }
        [field: SerializeField] public SaintsDictionary<ItemData, int> Items { get; private set; }
    }
}
