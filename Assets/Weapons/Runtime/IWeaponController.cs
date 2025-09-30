using System;

namespace Weapons.Runtime {
    public interface IWeaponController {
        public void Possess(Weapon weapon, WeaponStats stats);
        
        /// <summary>
        /// Update the weapon controller with the given attack action.
        /// </summary>
        /// <param name="action">The attack action to update the controller with.</param>
        /// <returns>The time until the next attack can be performed.</returns>
        public float UpdateOnAttack(AttackAction action);
    }
}
