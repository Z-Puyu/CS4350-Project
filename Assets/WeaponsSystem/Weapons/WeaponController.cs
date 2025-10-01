using System;
using GameplayAbilities.Runtime.Attributes;

namespace WeaponsSystem.Weapons {
    [Serializable]
    public abstract class WeaponController : IWeaponController {
        protected Weapon Weapon { get; private set; }
        protected AttributeSet Stats { get; private set; }
        
        public virtual void Possess(Weapon weapon, AttributeSet stats) {
            this.Weapon = weapon;
            this.Stats = stats;
        }

        public abstract void UpdateOnAttack();
        public abstract void UpdatePostAttack();
    }
}
