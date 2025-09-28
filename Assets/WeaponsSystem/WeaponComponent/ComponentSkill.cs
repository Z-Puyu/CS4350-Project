using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.Abilities;
using SaintsField;
using Unity.VisualScripting;
using UnityEngine;

namespace WeaponsSystem.WeaponComponent {
    [Serializable]
    public struct ComponentSkill {
        [field: SerializeField] public SaintsHashSet<WeaponComponentData> Components { get; set; }
        
        [field: SerializeField, Dropdown(nameof(this.AllSkillId))] 
        public string SkillId { get; set; }

        private DropdownList<string> AllSkillId =>
                new DropdownList<string>(PerkDatabase.GetAllObtainableAbilityIds().Select(id => (id, id)));
    }
}
