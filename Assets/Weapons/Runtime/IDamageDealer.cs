using System.Collections.Generic;
using UnityEngine;
using WeaponsSystem.DamageHandling;

namespace Weapons.Runtime {
    public interface IDamageDealer {
        public Damage DealDamage();
    }
}
