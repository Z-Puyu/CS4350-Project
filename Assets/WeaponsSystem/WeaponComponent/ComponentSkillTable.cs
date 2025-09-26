using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.WeaponComponent {
    [CreateAssetMenu(fileName = "ComponentSkillTable", menuName = "Weapons/Components/Component Skill Table", order = 1)]
    public class ComponentSkillTable : ScriptableObject, IEnumerable<KeyValuePair<HashSet<WeaponComponentData>, string>> {
        [field: SerializeField, SaintsDictionary("Component Combination", "Skill Id")]
        private SaintsDictionary<List<WeaponComponentData>, string> Table { get; set; }
            = new SaintsDictionary<List<WeaponComponentData>, string>();
        
        public IEnumerator<KeyValuePair<HashSet<WeaponComponentData>, string>> GetEnumerator() {
            return this.Table.Select(kvp => new KeyValuePair<HashSet<WeaponComponentData>, string>(new HashSet<WeaponComponentData>(kvp.Key), kvp.Value)).GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
