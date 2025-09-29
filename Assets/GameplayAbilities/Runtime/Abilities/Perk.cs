using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities {
    [CreateAssetMenu(fileName = "New Perk", menuName = "Gameplay Abilities/Perk")]
    public sealed class Perk : ScriptableObject {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public List<Perk> Prerequisites { get; private set; } = new List<Perk>();
        [field: SerializeField, Table] public List<PerkModifier> Modifiers { get; private set; } = new List<PerkModifier>();
        [field: SerializeField] public List<Ability> Abilities { get; private set; } = new List<Ability>();
    }
}
