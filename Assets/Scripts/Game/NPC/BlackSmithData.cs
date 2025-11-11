using GameplayAbilities.Runtime.Attributes;
using WeaponsSystem.Runtime.WeaponComponents;
using SaintsField;
using System.Collections.Generic;
using UnityEngine;
using Shop.Runtime;

namespace Game.NPC
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New NPC Data", menuName = "NPC/Blacksmith Data")]
    public class BlackSmithData : ScriptableObject
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public AttributeTable Attributes { get; private set; }
        [field: SerializeField] public AnimatorOverrideController Animations { get; private set; }
        [Header("Blacksmith Inventory")]
        [field: SerializeField] public SaintsDictionary<WeaponComponent, int> Items { get; private set; }

    }

}
