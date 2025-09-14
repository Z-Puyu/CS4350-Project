using System;
using SaintsField;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player_related.Player {
    [Obsolete]
    public class PlayerMovement : MonoBehaviour {
        [field: SerializeField, MinValue(0)] private float SpeedMultiplier { get; set; } = 5;
        private Vector2 Velocity { get; set; }
        [field: SerializeField] private Rigidbody2D Body { get; set; }
        
        public void MoveTowards(Vector2 direction) {
            this.Velocity = direction * this.SpeedMultiplier;
        }

        public void Stop() {
            this.Velocity = Vector2.zero;
        }

        private void Update() {
            this.Body.position += this.Velocity * Time.deltaTime;
        }
        
        // Sprite Rendering
        public bool IsWalking => this.Velocity != Vector2.zero;
        public Vector2 CurrentDirection => this.Velocity.normalized;
    }
}
