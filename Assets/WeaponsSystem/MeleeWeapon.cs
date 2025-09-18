using Common;

namespace WeaponsSystem {
    public sealed class MeleeWeapon : Weapon {
        public override void Attack() { 
            OnScreenDebugger.Log("Melee Attack");
            this.StartAttack();
            this.EndAttack();
        }
    }
}
    