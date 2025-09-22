using System.Collections.Generic;
using System.Linq;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace WeaponsSystem.DamageHandling {
    /// <summary>
    /// This component is responsible for triggering and managing combat animations,
    /// and invoke damage logic in the weapons.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Combatant : MonoBehaviour {
        [field: SerializeField] private LayerMask EnemyLayerMask { get; set; }
        [field: SerializeField, Tag] private List<string> EnemyTags { get; set; } = new List<string>();
        [field: SerializeField] private Animator Animator { get; set; }
        
        [field: SerializeField, ShowIf(nameof(this.Animator))]
        [field: AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Trigger)]
        private int AnimatorAttackTrigger { get; set; }
        
        [field: SerializeField, ShowIf(nameof(this.Animator))]
        [field: AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Int)]
        private int AnimatorComboCounter { get; set; }
        
        private IDamageDealer DamageDealer { get; set; }
        private bool IsAttacking { get; set; }

        public void StartAttack() {
            if (this.IsAttacking) {
                return;
            }
            
            this.IsAttacking = true;
            this.Animator.SetTrigger(this.AnimatorAttackTrigger);
            int combo = this.DamageDealer.StartAttack();
            this.Animator.SetInteger(this.AnimatorComboCounter, combo);
        }

        public void DealDamage(Vector3 forward) {
            this.DamageDealer.DealDamage(this.EnemyTags, this.EnemyLayerMask, forward);
        }

        public void FinishAttack() {
            this.IsAttacking = false;
            this.DamageDealer.EndAttack();
        }
        
        public void Equip(IDamageDealer damageDealer) {
            this.DamageDealer = damageDealer;
            this.DamageDealer.ConnectComboResetEvent(this.ResetCombo);
            this.IsAttacking = false;
            this.Animator.SetInteger(this.AnimatorComboCounter, 0);
            this.Animator.ResetTrigger(this.AnimatorAttackTrigger);
        }

        private void ResetCombo() {
            this.Animator.SetInteger(this.AnimatorComboCounter, 0);
        }
    }
}
