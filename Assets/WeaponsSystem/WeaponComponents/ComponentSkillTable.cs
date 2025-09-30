using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.WeaponComponents {
    [CreateAssetMenu(fileName = "ComponentSkillTable", menuName = "Weapons/Components/Component Skill Table", order = 1)]
    public class ComponentSkillTable : ScriptableObject, IEnumerable<KeyValuePair<ISet<AttributeBasedWeaponComponent>, string>> {
        [field: SerializeField, Table]
        private List<ComponentSkill> Table { get; set; } = new List<ComponentSkill>();
        
        public IEnumerator<KeyValuePair<ISet<AttributeBasedWeaponComponent>, string>> GetEnumerator() {
            return this.Table.Select(entry =>
                    new KeyValuePair<ISet<AttributeBasedWeaponComponent>, string>(entry.Components, entry.SkillId)
            ).GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
