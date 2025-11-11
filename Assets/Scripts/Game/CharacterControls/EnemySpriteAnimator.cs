using System;
using SaintsField;
using UnityEngine;

namespace Game.CharacterControls {
    [DisallowMultipleComponent, RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class EnemySpriteAnimator : MonoBehaviour {
        [field: SerializeField, Required] private Transform RootTransform { get; set; }
        [field: SerializeField, Required] private Movement MovementComponent { get; set; }
        [field: SerializeField] private int PixelOffset { get; set; }
        
        [field: SerializeField, AnimatorParam(AnimatorControllerParameterType.Bool)] 
        private int AnimatorMovementFlag { get; set; } 
        
        [field: SerializeField, AnimatorParam(AnimatorControllerParameterType.Trigger)] 
        private int AnimatorAttackTrigger { get; set; }

        private Animator Animator { get; set; }
        private SpriteRenderer SpriteRenderer { get; set; }
        [field: SerializeField] private PlayerTracker PlayerTracker { get; set; }

        private void Awake() {
            this.Animator = this.GetComponent<Animator>();
            this.SpriteRenderer = this.GetComponent<SpriteRenderer>();
        }

        private void Start() {
            float offset = this.PixelOffset / this.SpriteRenderer.sprite.pixelsPerUnit;
            this.transform.Translate(Vector3.right * offset, Space.Self);
        }

        private void Update() {
            // Movement Animation (still driven by keyboard movement)
            this.Animator.SetBool(this.AnimatorMovementFlag, this.MovementComponent.IsMoving);

            if (this.PlayerTracker.shouldFlip) {
                Vector3 scale = this.RootTransform.localScale;
                this.RootTransform.localScale = new Vector3(-1 * Math.Abs(scale.x), scale.y, scale.z);
            }
            else {
                Vector3 scale = this.RootTransform.localScale;
                this.RootTransform.localScale = new Vector3(Math.Abs(scale.x), scale.y, scale.z);
            }
        }
    }
}