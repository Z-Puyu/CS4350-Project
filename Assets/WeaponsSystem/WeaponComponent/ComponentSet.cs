using System.Collections;
using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.WeaponComponent {
    [CreateAssetMenu(fileName = "Component Set", menuName = "Weapons/Component Set")]
    public sealed class ComponentSet : ScriptableObject, IEnumerable<WeaponComponentData> {
        [field: SerializeField]
        private List<WeaponComponentData> Components { get; set; } = new List<WeaponComponentData>();

        public IEnumerator<WeaponComponentData> GetEnumerator() {
            return this.Components.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
