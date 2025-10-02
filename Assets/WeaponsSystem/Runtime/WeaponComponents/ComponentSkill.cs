using System;
using System.Linq;
using GameplayAbilities.Runtime.Abilities;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.Runtime.WeaponComponents {
    [Serializable]
    public struct ComponentSkill {
        [field: SerializeField] public SaintsHashSet<WeaponComponent> Components { get; set; }
        
        [field: SerializeField, Dropdown(nameof(this.AllSkillId))] 
        public string SkillId { get; set; }

        private DropdownList<string> AllSkillId =>
                new DropdownList<string>(PerkDatabase.GetAllObtainableAbilityIds().Select(id => (id, id)));
    }
}
