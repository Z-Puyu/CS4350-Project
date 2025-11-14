using System;
using SaintsField;
using UnityEngine;

namespace Game.CharacterControls {
    [DisallowMultipleComponent, RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class FarmerSpriteAnimator : MonoBehaviour {
        [field: SerializeField, Required] private Transform RootTransform { get; set; }
        [field: SerializeField, Required] private Movement MovementComponent { get; set; }
        [field: SerializeField] private int PixelOffset { get; set; }
        
        [field: SerializeField, AnimatorParam(AnimatorControllerParameterType.Bool)] 
        private int AnimatorMovementFlag { get; set; } 
        
        [field: SerializeField, AnimatorParam(AnimatorControllerParameterType.Trigger)] 
        private int AnimatorAttackTrigger { get; set; }
        
        [field: SerializeField, AnimatorParam(AnimatorControllerParameterType.Float)]
        private int AnimatorMoveX { get; set; }

        [field: SerializeField, AnimatorParam(AnimatorControllerParameterType.Float)]
        private int AnimatorMoveY { get; set; }

        [field: SerializeField, AnimatorParam(AnimatorControllerParameterType.Float)]
        private int AnimatorLastMoveX { get; set; }

        [field: SerializeField, AnimatorParam(AnimatorControllerParameterType.Float)]
        private int AnimatorLastMoveY { get; set; }
        
        [field: SerializeField, Required] private Transform WeaponSprite { get; set; }
        
        private Animator Animator { get; set; }
        private SpriteRenderer SpriteRenderer { get; set; }
        
        private Vector2 lastInput;
        private bool useMouseForFacing = false;
        
        public Vector2 LastDashDirection { get; set; }

        private void Awake() {
            this.Animator = this.GetComponent<Animator>();
            this.SpriteRenderer = this.GetComponent<SpriteRenderer>();
        }

        private void Start() {
            float offset = this.PixelOffset / this.SpriteRenderer.sprite.pixelsPerUnit;
            this.transform.Translate(Vector3.right * offset, Space.Self);
        }
        
        public void SetMoveInput(Vector2 input) => lastInput = input;

        private void Update() {
            // --- Movement flag ---
            bool isMoving = this.MovementComponent.IsMoving;
            this.Animator.SetBool(this.AnimatorMovementFlag, isMoving);
            
            LookAndBlendByMouse();
        }
        
        // private void LateUpdate() {
        //     if (WeaponSprite == null || RootTransform == null)
        //         return;
        //
        //     // --- 1. Calculate direction toward mouse ---
        //     Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //     mouseWorld.z = WeaponSprite.position.z;
        //     Vector3 dir = (mouseWorld - WeaponSprite.position).normalized;
        //
        //     // --- 2. Calculate base angle ---
        //     float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //
        //     // --- 3. Mirror angle if character is flipped ---
        //     if (RootTransform.localScale.x < 0f) {
        //         angle = 180f - angle; // mirror horizontally
        //     }
        //
        //     WeaponSprite.rotation = Quaternion.Euler(0f, 0f, angle);
        // }
        
        private void LookAndBlendByMouse() {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 toMouse = (mouseWorld - this.RootTransform.position);
            toMouse.Normalize();

            // Blend tree parameters
            this.Animator.SetFloat(this.AnimatorMoveX, toMouse.x);
            this.Animator.SetFloat(this.AnimatorMoveY, toMouse.y);

            // Last move for idle
            if (this.MovementComponent.IsMoving) {
                this.Animator.SetFloat(this.AnimatorLastMoveX, toMouse.x);
                this.Animator.SetFloat(this.AnimatorLastMoveY, toMouse.y);
            }

            // Flip sprite
            if (Mathf.Abs(toMouse.x) > 0.01f) {
                int xScale = toMouse.x < 0 ? -1 : 1;
                Vector3 scale = this.RootTransform.localScale;
                SpriteRenderer.gameObject.transform.localScale = new Vector3(xScale * Mathf.Abs(scale.x), scale.y, scale.z);
            }
        }
    }
}