using SaintsField;
using UnityEngine;

namespace Game.CharacterControls {
    [DisallowMultipleComponent, RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class SpriteAnimator : MonoBehaviour {
        [field: SerializeField, Required] private Movement MovementComponent { get; set; }
        
        [field: SerializeField, AnimatorParam(AnimatorControllerParameterType.Bool)] 
        private int AnimatorMovementFlag { get; set; } 
        
        [field: SerializeField, AnimatorParam(AnimatorControllerParameterType.Trigger)] 
        private int AnimatorAttackTrigger { get; set; }

        private Animator Animator { get; set; }
        private SpriteRenderer SpriteRenderer { get; set; }

        private bool IsAttacking { get; set; }
        private bool IsComboPossible { get; set; }

        private void Awake() {
            this.Animator = this.GetComponent<Animator>();
            this.SpriteRenderer = this.GetComponent<SpriteRenderer>();
        }

        private void Update() {
            // Movement Animation
            this.Animator.SetBool(this.AnimatorMovementFlag, this.MovementComponent.IsMoving);
            this.SpriteRenderer.flipX = this.MovementComponent.TargetDirection.x switch {
                // Flip based on the horizontal direction
                < 0 => true,
                > 0 => false,
                var _ => this.SpriteRenderer.flipX
            };
        }

        public void PlayAttack() {
            if (!this.IsAttacking) {
                // First Attack
                this.Animator.SetTrigger(this.AnimatorAttackTrigger);
                this.IsAttacking = true;
                this.IsComboPossible = true;
            } else if (this.IsComboPossible) {
                // Queue Combo
                this.Animator.SetTrigger(this.AnimatorAttackTrigger);
                this.IsComboPossible = false;
            }
        }

        public void AttackAnimationEnded() {
            this.IsAttacking = false;
            this.IsComboPossible = false;
        }
    }
}