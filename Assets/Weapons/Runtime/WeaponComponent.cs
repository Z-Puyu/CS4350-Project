using UnityEngine;

namespace Weapons.Runtime {
    public class WeaponComponent : ScriptableObject {
        public virtual void Modify(Weapon weapon, WeaponStats stats) { }
        
        public virtual void ModifyAttack(Weapon weapon, WeaponStats stats, int comboIndex) { }
        
        public virtual void UndoEffects(Weapon weapon, WeaponStats stats) { }
        
        public virtual void UndoAttackEffects(Weapon weapon, WeaponStats stats, int comboIndex) { }
    }
}
