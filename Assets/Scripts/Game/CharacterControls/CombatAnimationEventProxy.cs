using System;
using SaintsField;
using SaintsField.Samples.Scripts.IssueAndTesting.Issue.Issue8;
using UnityEngine;
using WeaponsSystem.Runtime.Combat;

namespace Game.CharacterControls {
    [DisallowMultipleComponent, RequireComponent(typeof(Animator))]
    public sealed class CombatAnimationEventProxy : MonoBehaviour {
        private Animator Animator { get; set; }
        [field: SerializeField, Required] private Combatant Combatant { get; set; }
        [field: SerializeField, Required] private Transform RootTransform { get; set; }
        [field: SerializeField, Required] private GameCharacterController GameCharacterController { get; set; }
        
        [field: SerializeField, AnimatorParam(nameof(this.GetAnimator), AnimatorControllerParameterType.Bool)]
        private int AnimatorDeathFlag { get; set; }
        
        private Animator GetAnimator() {
            return this.Animator ? this.Animator : this.Animator = this.GetComponent<Animator>();
        }

        private void Awake() {
            this.Animator = this.GetComponent<Animator>();
        }

        public void OnAttackEvent() {
            Vector3 forward = Math.Sign(this.RootTransform.localScale.x) * Vector3.right;
            this.Combatant.PerformAttack(forward);
        }

        public void OnAttackEndEvent() {
            this.Combatant.QueryFinishAttack();
        }

        public void OnDeathEvent() {
            this.GetAnimator().SetBool(this.AnimatorDeathFlag, false);
            this.GameCharacterController.Bury();
        }
    }
}
