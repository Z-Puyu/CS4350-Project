using System;
using System.Collections.Generic;
using SaintsField;
using UnityEngine;
using WeaponsSystem;

namespace WeaponsSystem.WeaponComponent {
    [Serializable, CreateAssetMenu(fileName = "ComponentData", menuName = "Weapons/Components/ComponentData", order = 0)]
    public class WeaponComponentData : ScriptableObject {
        [field: SerializeField, Table] public List<ComponentModifierData> modifiers = new List<ComponentModifierData>();
        [field: SerializeField] private int id;
    }
}
