using System.Collections.Generic;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities {
    [CreateAssetMenu(fileName = "New Perk", menuName = "Gameplay Abilities/Perk")]
    public class Perk : ScriptableObject {
        [field: SerializeField, Table] private List<PerkModifier> Modifiers { get; set; } = new List<PerkModifier>();
        [field: SerializeField] private List<Ability> Abilities { get; set; } = new List<Ability>();

        public void Enable(AttributeSet target) {
            this.Modifiers.ForEach(modifier => target.AddModifier(modifier.ToModifier()));
        }
    }
}
