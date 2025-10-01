using System.Collections.Generic;
using GameplayAbilities.Runtime.Abilities;
using UnityEngine;

namespace WeaponsSystem.Runtime.WeaponComponents {
    public sealed class ComponentSkillTester : MonoBehaviour {
        [field: SerializeField] private ComponentSkillTable SkillTable { get; set; }
        [field: SerializeField] private AbilitySystem AbilitySystem { get; set; }
        [field: SerializeField] private AbilityCaster AbilityCaster { get; set; }

        public void Test(ISet<WeaponComponent> components) {
            foreach (KeyValuePair<ISet<WeaponComponent>, string> entry in this.SkillTable) {
                if (!entry.Key.IsSubsetOf(components)) {
                    continue;
                }
#if DEBUG
                Debug.Log($"Component Skill Tester Activating skill {entry.Value}", this);
#endif
                Ability ability = this.AbilitySystem.Grant(entry.Value);
                
                //TODO: Use UI to change this:
                this.AbilityCaster.Equip(ability, 0);
            }
        }
    }
}
