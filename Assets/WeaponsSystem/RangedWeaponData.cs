using UnityEngine;

namespace WeaponsSystem {
    [CreateAssetMenu(fileName = "RangedWeaponData", menuName = "Weapons/RangedWeaponData", order = 0)]
    public class RangedWeaponData : WeaponData {
        [filed: SerializeField] public float FireRate { get; private set; }
        [filed: SerializeField] public float Range { get; private set; }
        [filed: SerializeField] public float BulletSpeed { get; private set; }
        [filed: SerializeField] public float MagSize { get; private set; }
        [filed: SerializeField] public float ReloadTime { get; private set; } //is this required?
        [filed: SerializeField] public float Spread { get; private set; }
    }
}
