using UnityEngine;

namespace Game.CharacterControls {
    public class Movement2D : Movement {
        [field: SerializeField] private Rigidbody2D RootRigidBody { get; set; }

        protected override void Move() {
            if (this.RootRigidBody) {
                this.RootRigidBody.MovePosition(this.RootRigidBody.position + (Vector2)this.CurrentVelocity);
            } else {
                base.Move();
            }
        }
    }
}
