using System;
using GameplayAbilities.Runtime.Attributes;
using WeaponsSystem.Runtime.Attacks;

namespace WeaponsSystem.Runtime.Weapons {
    [Serializable]
    public abstract class WeaponController : IWeaponController {
        protected Weapon Weapon { get; private set; }
        protected AttributeSet Stats { get; private set; }
        
        public virtual void Possess(Weapon weapon, AttributeSet stats) {
            this.Weapon = weapon;
            this.Stats = stats;
        }

        public abstract void UpdateOnAttack(ref AttackAction action);
        public abstract void UpdatePostAttack();
    }
}
