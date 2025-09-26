using System;
using UnityEngine;

namespace WeaponsSystem {
    [Serializable]
    public class RangedAttackData : AttackData {
        [field: SerializeField]
        private RangedWeapon.ProjectileSpawnMethod FireMode { get; set; } = RangedWeapon.ProjectileSpawnMethod.Single;
    }
}
