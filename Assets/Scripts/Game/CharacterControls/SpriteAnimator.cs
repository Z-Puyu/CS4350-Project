using System;
using SaintsField;
using UnityEngine;

namespace Game.CharacterControls {
    [DisallowMultipleComponent, RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class SpriteAnimator : MonoBehaviour {
        [field: SerializeField, Required] private Transform RootTransform { get; set; }
        [field: SerializeField, Required] private Movement MovementComponent { get; set; }
        [field: SerializeField] private int PixelOffset { get; set; }
        
        [field: SerializeField, AnimatorParam(AnimatorControllerParameterType.Bool)] 
        private int AnimatorMovementFlag { get; set; } 
        
        [field: SerializeField, AnimatorParam(AnimatorControllerParameterType.Trigger)] 
        private int AnimatorAttackTrigger { get; set; }

        private Animator Animator { get; set; }
        private SpriteRenderer SpriteRenderer { get; set; }

        private void Awake() {
            this.Animator = this.GetComponent<Animator>();
            this.SpriteRenderer = this.GetComponent<SpriteRenderer>();
        }

        private void Start() {
            float offset = this.PixelOffset / this.SpriteRenderer.sprite.pixelsPerUnit;
            this.transform.Translate(Vector3.right * offset, Space.Self);
        }

        private void Update() {
            // Movement Animation
            this.Animator.SetBool(this.AnimatorMovementFlag, this.MovementComponent.IsMoving);
            int xScale = this.MovementComponent.TargetDirection.x switch {
                // Flip based on the horizontal direction
                < 0 => -1,
                > 0 => 1,
                var _ => Math.Sign(this.RootTransform.localScale.x)
            };

            if (xScale == Math.Sign(this.RootTransform.localScale.x)) {
                return;
            }
            
            Vector3 scale = this.RootTransform.localScale;
            this.RootTransform.localScale = new Vector3(xScale * Math.Abs(scale.x), scale.y, scale.z);
            /*this.SpriteRenderer.flipX = this.MovementComponent.TargetDirection.x switch {
                // Flip based on the horizontal direction
                < 0 => true,
                > 0 => false,
                var _ => this.SpriteRenderer.flipX
            };*/
        }
    }
}