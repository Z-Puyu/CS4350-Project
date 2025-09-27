using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.WeaponComponent {
    [CreateAssetMenu(fileName = "ComponentSkillTable", menuName = "Weapons/Components/Component Skill Table", order = 1)]
    public class ComponentSkillTable : ScriptableObject, IEnumerable<KeyValuePair<ISet<WeaponComponentData>, string>> {
        [field: SerializeField, Table]
        private List<ComponentSkill> Table { get; set; } = new List<ComponentSkill>();
        
        public IEnumerator<KeyValuePair<ISet<WeaponComponentData>, string>> GetEnumerator() {
            return this.Table.Select(entry =>
                    new KeyValuePair<ISet<WeaponComponentData>, string>(entry.Components, entry.SkillId)
            ).GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
