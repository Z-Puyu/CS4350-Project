using UnityEngine;

namespace WeaponsSystem {
    [CreateAssetMenu(fileName = "RangedWeaponData", menuName = "Weapons/RangedWeaponData", order = 0)]
    public class RangedWeaponData : WeaponData {
        [field: SerializeField] public float FireInterval { get; private set; } //in milliseconds
        [field: SerializeField] public float Range { get; private set; }
        [field: SerializeField] public float BulletSpeed { get; private set; }
        [field: SerializeField] public float Spread { get; private set; }
    }
}
