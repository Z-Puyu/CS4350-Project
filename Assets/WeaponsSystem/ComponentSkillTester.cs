using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.Abilities;
using UnityEngine;
using WeaponsSystem.WeaponComponents;

namespace WeaponsSystem {
    public sealed class ComponentSkillTester : MonoBehaviour {
        [field: SerializeField] private ComponentSkillTable SkillTable { get; set; }
        [field: SerializeField] private AbilitySystem AbilitySystem { get; set; }

        public void Test(ISet<AttributeBasedWeaponComponent> components) {
            foreach (KeyValuePair<ISet<AttributeBasedWeaponComponent>, string> entry in this.SkillTable) {
                if (!entry.Key.IsSubsetOf(components)) {
                    continue;
                }
#if DEBUG
                Debug.Log($"Component Skill Tester Activating skill {entry.Value}", this);
#endif
                this.AbilitySystem.Grant(entry.Value);
            }
        }
    }
}
