using System;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.DamageHandling {
    [Serializable]
    public sealed class DamageDefinition {
        internal DropdownList<string> ModifierLabels { private get; set; }
        [field: SerializeField] public AttributeTypeDefinition DamageType { get; set; }
        [field: SerializeField] public string ModifierLabel { get; set; }
    }
}
