using System.Collections.Generic;
using Weapons.Runtime;

namespace WeaponsSystem.WeaponComponents {
    public sealed class AttributeBasedWeaponComponentController : WeaponComponentController {
        private WeaponAttributeStats Stats { get; }
        /*private void ActivateComponentSkills() {
            foreach (KeyValuePair<ISet<WeaponComponentData>, string> entry in this.ComponentSkillTable) {
                if (!entry.Key.IsSubsetOf(this.Components)) {
                    continue;
                }

                Debug.Log($"Component Manager Activating skill {entry.Value}", this);
                this.OnComponentSetChanged?.Invoke(entry.Key);
            }
        }

        private IEnumerator DelayedActivateSkills(float delay) {
            yield return new WaitForSeconds(delay);
            this.ActivateComponentSkills();
        }*/
        public AttributeBasedWeaponComponentController(
            Weapon weapon, int capacity, IEnumerable<WeaponComponent> possibleComponents, WeaponAttributeStats stats
        ) : base(weapon, capacity, possibleComponents) {
            this.Stats = stats;
        }
    }
}
