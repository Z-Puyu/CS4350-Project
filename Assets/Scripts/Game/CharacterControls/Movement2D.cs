using System.Collections.Generic;
using UnityEngine;

namespace Game.CharacterControls {
    public class Movement2D : Movement {
        [field: SerializeField] private Rigidbody2D RootRigidBody { get; set; }

        private Vector2 CastUntilBlocked(Vector2 displacement) {
            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            int hitCount = this.RootRigidBody.Cast(
                this.TargetDirection, new ContactFilter2D {
                    useTriggers = false,
                    useLayerMask = true,
                    layerMask = Physics2D.GetLayerCollisionMask(this.gameObject.layer)
                }, hits, displacement.magnitude
            );

            RaycastHit2D nearest = default;
            float distance = displacement.magnitude;
            for (int i = 0; i < hitCount; i += 1) {
                RaycastHit2D h = hits[i];
                if (!h.collider || h.collider.attachedRigidbody == this.RootRigidBody || h.distance >= distance) {
                    continue;
                }
                    
                distance = h.distance;
                nearest = h;
            }
            
            return nearest.point - this.RootRigidBody.position;
        }

        protected override void Move() {
            if (this.RootRigidBody) {
                Vector2 displacement = this.CurrentVelocity;
                
                
                this.RootRigidBody.MovePosition(this.RootRigidBody.position + (Vector2)this.CurrentVelocity);
            } else {
                base.Move();
            }
        }
    }
}
