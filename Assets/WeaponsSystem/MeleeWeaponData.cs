using UnityEngine;

namespace WeaponsSystem {
    [CreateAssetMenu(fileName = "MeleeWeaponData", menuName = "Weapons/MeleeWeaponData", order = 0)]
    public class MeleeWeaponData : WeaponData {
        [field: SerializeField] public float attackRange;
        [field: SerializeField] public float attackAngle; //radians
    }
}
