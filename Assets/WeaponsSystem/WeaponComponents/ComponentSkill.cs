using System;
using System.Linq;
using GameplayAbilities.Runtime.Abilities;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.WeaponComponents {
    [Serializable]
    public struct ComponentSkill {
        [field: SerializeField] public SaintsHashSet<AttributeBasedWeaponComponent> Components { get; set; }
        
        [field: SerializeField, Dropdown(nameof(this.AllSkillId))] 
        public string SkillId { get; set; }

        private DropdownList<string> AllSkillId =>
                new DropdownList<string>(PerkDatabase.GetAllObtainableAbilityIds().Select(id => (id, id)));
    }
}
