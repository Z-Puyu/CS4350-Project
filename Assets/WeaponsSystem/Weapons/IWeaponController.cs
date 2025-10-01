using GameplayAbilities.Runtime.Attributes;

namespace WeaponsSystem.Weapons {
    public interface IWeaponController {
        public void Possess(Weapon weapon, AttributeSet stats);
        
        public void UpdateOnAttack();
        
        public void UpdatePostAttack();
    }
}
