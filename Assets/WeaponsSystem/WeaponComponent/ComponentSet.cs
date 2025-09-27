using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.WeaponComponent {
    [CreateAssetMenu(fileName = "Component Set", menuName = "Weapons/Component Set")]
    public sealed class ComponentSet : ScriptableObject {
        [field: SerializeField, ValidateInput(nameof(this.ValidateComponents))]
        public List<WeaponComponentData> Components { get; private set; } = new List<WeaponComponentData>();

        private string ValidateComponents() {
            return this.Components.Count <= 64 ? string.Empty : "Too many components, maximum is 64.";
        }
    }
}
