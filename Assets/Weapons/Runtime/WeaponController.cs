using System;

namespace Weapons.Runtime {
    [Serializable]
    public abstract class WeaponController : IWeaponController {
        protected Weapon Weapon { get; private set; }
        protected WeaponStats Stats { get; private set; }
        
        public void Possess(Weapon weapon, WeaponStats stats) {
            this.Weapon = weapon;
            this.Stats = stats;
        }

        public abstract float UpdateOnAttack(AttackAction action);
    }
}
