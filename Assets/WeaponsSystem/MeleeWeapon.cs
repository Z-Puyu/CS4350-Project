using System;
using Common;
using UnityEngine;

namespace WeaponsSystem {
    public class MeleeWeapon : Weapon<MeleeWeaponData>{
        protected override void Awake() {
            base.Awake();
        }
        

        public override void Attack() { 
            OnScreenDebugger.Log("Melee Attack");
            this.StartAttack();
            this.EndAttack();
        }
    }
}
    