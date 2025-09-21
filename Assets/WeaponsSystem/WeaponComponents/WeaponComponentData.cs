using SaintsField;
using UnityEngine;

namespace WeaponsSystem.WeaponComponents {
    [CreateAssetMenu(fileName = "ComponentData", menuName = "Weapons/Components/ComponentData", order = 0)]
    public class WeaponComponentData : ScriptableObject {
        [SaintsDictionary] public SaintsDictionary<string, int> effects;
    }
}
