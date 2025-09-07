using UnityEngine;

namespace WeaponsSystem {
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/WeaponData", order = 0)]
    public class WeaponData : ScriptableObject {
        [field: SerializeField] public string weaponName;
        [field: SerializeField] public float damage;
        [field: SerializeField] public int comboLength;
        [field: SerializeField] public float  comboResetTime;
    }
}
