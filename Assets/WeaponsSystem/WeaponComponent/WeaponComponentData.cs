using System;
using System.Collections.Generic;
using SaintsField;
using Unity.Collections;
using UnityEngine;

namespace WeaponsSystem.WeaponComponent {
    [CreateAssetMenu(fileName = "ComponentData", menuName = "Weapons/Components/ComponentData", order = 0)]
    public class WeaponComponentData : ScriptableObject {
        [SaintsDictionary] public SaintsDictionary<string, int> effects;
    }
}
