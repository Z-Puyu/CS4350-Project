using System.Collections.Generic;
using Common;
using GameplayAbilities.Runtime.Abilities;
using UnityEngine;
using WeaponsSystem.DamageHandling;

namespace WeaponsSystem {
    [RequireComponent(typeof(DamageDealer))]
    public sealed class MeleeWeapon : Weapon<MeleeWeaponData>{
        private DamageDealer DamageDealer { get; set; }
        
        protected override void Awake() {
            base.Awake();
            this.DamageDealer = this.GetComponent<DamageDealer>();
        }

        private IDictionary<string, int> FetchDamageData() {
            return new Dictionary<string, int>();
        }

        private void ApplyDamage(AbilitySystem target) {
            this.DamageDealer.Damage(target, this.FetchDamageData());
        }
        
        public override void Attack() { 
            OnScreenDebugger.Log("Melee Attack");
            this.StartAttack();
            this.EndAttack();
        }
    }
}
    