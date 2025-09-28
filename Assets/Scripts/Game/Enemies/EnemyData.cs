using GameplayAbilities.Runtime.Attributes;
using ModularItemsAndInventory.Runtime.LootContainers;
using Player_related.Player_exp;
using UnityEngine;

namespace Game.Enemies {
    [CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemies/Enemy Data")]
    public class EnemyData : ScriptableObject {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public EnemyExpObject EnemyExpObject { get; private set; }
        [field: SerializeField] public AnimatorOverrideController Animations { get; private set; }
        [field: SerializeField] public AttributeTable Attributes { get; private set; }
        [field: SerializeField] public LootTable LootTable { get; private set; }
    }
}
