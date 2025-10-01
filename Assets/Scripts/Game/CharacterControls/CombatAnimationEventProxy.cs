using System;
using SaintsField;
using UnityEngine;
using WeaponsSystem.Runtime.Combat;

namespace Game.CharacterControls {
    [DisallowMultipleComponent]
    public sealed class CombatAnimationEventProxy : MonoBehaviour {
        [field: SerializeField, Required] private Combatant Combatant { get; set; }
        [field: SerializeField, Required] private Transform RootTransform { get; set; }
        [field: SerializeField, Required] private GameCharacterController GameCharacterController { get; set; }
        
        public void OnAttackEvent() {
            Vector3 forward = Math.Sign(this.RootTransform.localScale.x) * Vector3.right;
            this.Combatant.PerformAttack(forward);
        }

        public void OnAttackEndEvent() {
            this.Combatant.QueryFinishAttack();
        }

        public void OnDeathEvent() {
            this.GameCharacterController.Bury();
        }
    }
}
