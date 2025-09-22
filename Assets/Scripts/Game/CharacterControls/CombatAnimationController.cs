using System;
using SaintsField;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.CharacterControls {
    [DisallowMultipleComponent, RequireComponent(typeof(Animator))]
    public sealed class CombatAnimationController : MonoBehaviour {
        private Animator Animator { get; set; }
        
        [field: SerializeField] private int MaxCombo { get; set; } = 3;
        [field: SerializeField] private int AnimationCycleLength { get; set; } = 2;
        
        [field: SerializeField, AnimatorParam(nameof(this.GetAnimator), AnimatorControllerParameterType.Int)]
        private int AnimatorComboCounter { get; set; }
        
        [field: SerializeField, AnimatorParam(nameof(this.GetAnimator), AnimatorControllerParameterType.Trigger)]
        private int AnimatorAttackTrigger { get; set; }
        
        [field: SerializeField, AnimatorParam(nameof(this.GetAnimator), AnimatorControllerParameterType.Trigger)]
        private int AnimatorTakeDamageTrigger { get; set; }
        
        [field: SerializeField, AnimatorParam(nameof(this.GetAnimator), AnimatorControllerParameterType.Trigger)]
        private int AnimatorDeathTrigger { get; set; }
        
        private Animator GetAnimator() {
            return this.Animator ? this.Animator : this.Animator = this.GetComponent<Animator>();
        }

        private void Awake() {
            this.Animator = this.GetComponent<Animator>();
        }
        
        public void PlayTakeDamageAnimation() {
            this.ResetCombo();
            this.Animator.ResetTrigger(this.AnimatorTakeDamageTrigger);
            this.Animator.SetTrigger(this.AnimatorTakeDamageTrigger);
        }
        
        public void PlayDeathAnimation() {
            this.ResetCombo();
            this.Animator.SetTrigger(this.AnimatorDeathTrigger);
        }

        public void PlayAttackAnimation(int combo) {
            if (combo >= this.MaxCombo - 1) {
                this.Animator.SetInteger(this.AnimatorComboCounter, this.MaxCombo - 1);
            } else {
                this.Animator.SetInteger(this.AnimatorComboCounter, combo % this.AnimationCycleLength);
            }
            
            this.Animator.SetTrigger(this.AnimatorAttackTrigger);
        }
        
        public void ResetCombo() {
            this.Animator.ResetTrigger(this.AnimatorAttackTrigger);
            this.Animator.SetInteger(this.AnimatorComboCounter, 0);
        }
    }
}
