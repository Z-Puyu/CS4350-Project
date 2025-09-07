using UnityEngine;

namespace WeaponsSystem {
    public class MeleeWeapon : Weapon{
        [field: SerializeField] public MeleeWeaponData WeaponData { get; private set; }
        private BoxCollider2D weaponCollider;
        
        
        public void Awake() {
            this.weaponCollider = new BoxCollider2D();
            this.weaponCollider.size = new Vector2(1, this.WeaponData.attackRange);
            this.weaponCollider.offset = new Vector2(0, this.WeaponData.attackRange / 2);
        }
        
        public override void Attack() { 
            Debug.Log("Melee Attack");
        }
    }
}
