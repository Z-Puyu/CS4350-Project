using GameplayAbilities.Runtime.Attributes;
using WeaponsSystem.Runtime.Attacks;

namespace WeaponsSystem.Runtime.Weapons {
    public interface IWeaponController {
        public void Possess(Weapon weapon, AttributeSet stats);
        
        public void UpdateOnAttack(ref AttackAction action);
        
        public void UpdatePostAttack();
    }
}
