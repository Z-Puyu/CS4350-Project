using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.Abilities;
using UnityEngine;
using WeaponsSystem.WeaponComponent;

namespace WeaponsSystem {
    public sealed class ComponentSkillTester : MonoBehaviour {
        [field: SerializeField] private ComponentSkillTable SkillTable { get; set; }
        [field: SerializeField] private AbilitySystem AbilitySystem { get; set; }
        [field: SerializeField] private AbilityRoundRobin AbilityRoundRobin { get; set; }

        public void Test(ISet<WeaponComponentData> components) {
            foreach (KeyValuePair<ISet<WeaponComponentData>, string> entry in this.SkillTable) {
                if (!entry.Key.IsSubsetOf(components)) {
                    continue;
                }
#if DEBUG
                Debug.Log($"Component Skill Tester Activating skill {entry.Value}", this);
#endif
                this.AbilitySystem.Grant(entry.Value);
                
                //TODO: Use UI to change this:
                this.AbilityRoundRobin.Equip(PerkDatabase.GetAbility(entry.Value), 0);
            }
        }
    }
}
