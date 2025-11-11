using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities {
    [CreateAssetMenu(fileName = "New Perk", menuName = "Gameplay Abilities/Perk")]
    public sealed class Perk : ScriptableObject {

        public enum PerkType
        {
            Combat,
            Farming
        }
        
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Sprite sprite{ get; private set; }
        [field: SerializeField] public List<Perk> Prerequisites { get; private set; } = new List<Perk>();
        [field: SerializeField, Table] public List<PerkModifier> Modifiers { get; private set; } = new List<PerkModifier>();
        [field: SerializeField] public List<Ability> Abilities { get; private set; } = new List<Ability>();
        [field: SerializeField] public int skillPointsToUnlock{ get; private set; }
        [field: SerializeField] public PerkType perkType { get; private set; }

        
        public bool IsCombatPerk()
        {
            return perkType == PerkType.Combat;
        }
    }
}
