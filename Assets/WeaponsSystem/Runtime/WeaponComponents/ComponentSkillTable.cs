using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.Runtime.WeaponComponents {
    [CreateAssetMenu(fileName = "ComponentSkillTable", menuName = "Weapons/Components/Component Skill Table")]
    public class ComponentSkillTable : ScriptableObject, IEnumerable<KeyValuePair<ISet<WeaponComponent>, string>> {
        [field: SerializeField, Table]
        private List<ComponentSkill> Table { get; set; } = new List<ComponentSkill>();
        
        public IEnumerator<KeyValuePair<ISet<WeaponComponent>, string>> GetEnumerator() {
            return this.Table
                       .Select(pair => new KeyValuePair<ISet<WeaponComponent>, string>(pair.Components, pair.SkillId))
                       .GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
