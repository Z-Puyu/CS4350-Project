using SaintsField;
using UnityEngine;

namespace Game.CharacterControls {
    [DisallowMultipleComponent]
    public class Movement : MonoBehaviour {
        [field: SerializeField, MinValue(1)] private float Speed { get; set; } = 5;
        [field: SerializeField, Required] private Transform RootTransform { get; set; }
        public Vector3 TargetDirection { get; private set; }
        protected Vector3 CurrentVelocity { get; set; }
        
        public bool IsMoving => this.TargetDirection != Vector3.zero;
        
        public void MoveTo(GameObject target) {
            this.TargetDirection = (target.transform.position - this.RootTransform.position).normalized;
        }
        
        public virtual void MoveTo(Vector3 location) {
            this.TargetDirection = (location - this.RootTransform.position).normalized;
            this.CurrentVelocity = this.TargetDirection * (this.Speed * Time.fixedDeltaTime);
        }

        public void MoveIn(Vector3 direction) {
            this.TargetDirection = direction.normalized;       
        }
        
        public void Stop() {
            this.TargetDirection = Vector3.zero;
            this.CurrentVelocity = Vector3.zero;       
        }

        protected virtual void Move() {
            if (!this.IsMoving) {
                return;           
            }
            
            this.RootTransform.position += this.CurrentVelocity;
        }
        
        protected void FixedUpdate() {
            this.CurrentVelocity = this.TargetDirection * (this.Speed * Time.fixedDeltaTime);
            this.Move();
        }
    }
}
