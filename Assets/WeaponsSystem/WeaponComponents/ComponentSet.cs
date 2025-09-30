using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeaponsSystem.WeaponComponents {
    [CreateAssetMenu(fileName = "Component Set", menuName = "Weapons/Component Set")]
    public sealed class ComponentSet : ScriptableObject, IEnumerable<AttributeBasedWeaponComponent> {
        [field: SerializeField]
        private List<AttributeBasedWeaponComponent> Components { get; set; } = new List<AttributeBasedWeaponComponent>();

        public IEnumerator<AttributeBasedWeaponComponent> GetEnumerator() {
            return this.Components.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
