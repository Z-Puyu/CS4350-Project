using System;
using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace Game.CharacterControls {
    [RequireComponent(typeof(BoxCollider2D))]
    public class Movement2D : Movement {
        [field: SerializeField] private Rigidbody2D RootRigidBody { get; set; }
        
        [field: SerializeField, PropRange(0.01f, 1, 0.01f)] 
        private float Skin { get; set; } = 0.01f;
        
        private BoxCollider2D Collider { get; set; }

        private void Awake() {
            this.Collider = this.GetComponent<BoxCollider2D>();
        }

        private Vector2 CastUntilBlocked(Vector2 displacement) {
            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            int hitCount = this.Collider.Cast(
                this.TargetDirection, new ContactFilter2D {
                    useTriggers = false,
                    useLayerMask = true,
                    layerMask = Physics2D.GetLayerCollisionMask(this.gameObject.layer)
                }, hits, displacement.magnitude + this.Skin
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

            if (nearest.collider) {
                return (distance - this.Skin) * this.TargetDirection;
            }

            return displacement;
        }

        protected override void Move() {
            if (this.RootRigidBody) {
                Vector2 displacement = this.CurrentVelocity;
                Vector2 movement = this.CastUntilBlocked(displacement);
                if (movement == Vector2.zero) {
                    this.Stop();
                    return;
                }
                
                this.RootRigidBody.MovePosition(this.RootRigidBody.position + movement);
            } else {
                base.Move();
            }
        }
    }
}
