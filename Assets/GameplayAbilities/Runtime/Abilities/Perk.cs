using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities {
    [CreateAssetMenu(fileName = "New Perk", menuName = "Gameplay Abilities/Perk")]
    public sealed class Perk : ScriptableObject {
        [field: SerializeField, Table] internal List<PerkModifier> Modifiers { get; private set; } = new List<PerkModifier>();
        [field: SerializeField] public List<Ability> Abilities { get; private set; } = new List<Ability>();
    }
}
