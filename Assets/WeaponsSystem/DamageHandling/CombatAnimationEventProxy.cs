using System;
using SaintsField;
using UnityEngine;
using CharacterController = Game.CharacterControls.CharacterController;

namespace WeaponsSystem.DamageHandling {
    [DisallowMultipleComponent]
    public sealed class CombatAnimationEventProxy : MonoBehaviour {
        [field: SerializeField, Required] private Combatant Combatant { get; set; }
        [field: SerializeField, Required] private Transform RootTransform { get; set; }
        [field: SerializeField, Required] private CharacterController CharacterController { get; set; }
        
        public void OnAttackEvent() {
            Vector3 forward = Math.Sign(this.RootTransform.localScale.x) * Vector3.right;
            this.Combatant.DealDamage(forward);
        }

        public void OnAttackEndEvent() {
            this.Combatant.FinishAttack();
        }

        public void OnDeathEvent() {
            this.CharacterController.Bury();
        }
    }
}
