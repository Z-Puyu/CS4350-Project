using System.Collections.Generic;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace WeaponsSystem.DamageHandling {
    /// <summary>
    /// This component is responsible for triggering and managing combat animations,
    /// and invoke damage logic in the weapons.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Combatant : MonoBehaviour {
        [field: SerializeField] private LayerMask EnemyLayerMask { get; set; }
        [field: SerializeField, Tag] private List<string> EnemyTags { get; set; } = new List<string>();
        [field: SerializeField] private UnityEvent<int> OnAttacked { get; set; } = new UnityEvent<int>();

        [field: SerializeField]
        private UnityEvent<IDamageDealer> OnSwitchedGear { get; set; } = new UnityEvent<IDamageDealer>();
        
        private IDamageDealer DamageDealer { get; set; }
        private bool IsAttacking { get; set; }

        public void StartAttack() {
            if (this.IsAttacking) {
                return;
            }
            
            this.IsAttacking = true;
            int combo = this.DamageDealer.Attack();
            this.OnAttacked.Invoke(combo);
        }

        public void DealDamage(Vector3 forward) {
            this.DamageDealer.DealDamage(this.EnemyTags, this.EnemyLayerMask, forward);
        }

        public void FinishAttack() {
            this.IsAttacking = false;
        }
        
        public void Equip(IDamageDealer damageDealer) {
            if (damageDealer == this.DamageDealer) {
                return;           
            }
            
            this.DamageDealer = damageDealer;
            this.IsAttacking = false;
            this.OnSwitchedGear.Invoke(damageDealer);
        }
    }
}
