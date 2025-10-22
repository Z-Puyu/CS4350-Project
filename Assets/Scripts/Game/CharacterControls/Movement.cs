using System.Collections;
using UnityEngine;
using SaintsField;

namespace Game.CharacterControls {
    [DisallowMultipleComponent]
    public class Movement : MonoBehaviour {
        [field: SerializeField, MinValue(1)] private float Speed { get; set; } = 5;
        [field: SerializeField, Required] private Transform RootTransform { get; set; }

        [Header("Dash Settings")]
        [field: SerializeField, MinValue(0.01f)] private float DashDistance { get; set; } = 4f;
        [field: SerializeField, MinValue(0.01f)] private float DashDuration { get; set; } = 0.18f;
        [field: SerializeField, MinValue(0f)] private float DashCooldown { get; set; } = 0.8f;
        [field: SerializeField] private TrailRenderer DashTrail { get; set; }

        public Vector3 TargetDirection { get; private set; }
        protected Vector3 CurrentVelocity { get; set; }

        // dash state
        private bool isDashing = false;
        private float lastDashTime = -999f;

        // last meaningful input/facing
        private Vector2 lastMoveDirection = Vector2.down;

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
            // remember last non-zero direction for facing/dash
            if (direction.sqrMagnitude > 0.0001f) {
                lastMoveDirection = new Vector2(direction.x, direction.y).normalized;
            }
        }

        public void Stop() {
            this.TargetDirection = Vector3.zero;
            this.CurrentVelocity = Vector3.zero;
        }

        // --- new getters used by PlayerInputParser ---
        public Vector2 GetLastMoveDirection() => lastMoveDirection;
        public Vector2 GetFacingDirection() => lastMoveDirection; // alias, can be changed if you have a separate facing

        // --- dash API ---
        // Accepts Vector2 from input layer
        public void Dash(Vector2 direction)
        {
            // guard: cooldown and current dash state
            if (isDashing || Time.time - lastDashTime < DashCooldown)
                return;

            Vector2 dir = direction;
            if (dir.sqrMagnitude < 0.0001f)
                dir = lastMoveDirection;

            if (dir.sqrMagnitude < 0.0001f)
                dir = Vector2.down; // fallback

            StartCoroutine(DashRoutine(new Vector3(dir.x, dir.y, 0f).normalized));
        }

        private IEnumerator DashRoutine(Vector3 dir)
        {
            isDashing = true;
            lastDashTime = Time.time;

            Vector3 start = RootTransform.position;
            Vector3 end = start + dir * DashDistance;

            // Disable normal movement during dash
            Vector3 previousTarget = this.TargetDirection;
            this.TargetDirection = Vector3.zero;
            this.CurrentVelocity = Vector3.zero;

            // Start trail
            if (DashTrail)
            {
                DashTrail.Clear();
                DashTrail.emitting = true;
            }

            float elapsed = 0f;
            while (elapsed < DashDuration)
            {
                RootTransform.position = Vector3.Lerp(start, end, elapsed / DashDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            RootTransform.position = end;

            // Stop trail
            if (DashTrail)
            {
                DashTrail.emitting = false;
            }

            // Restore normal movement
            this.TargetDirection = previousTarget;
            isDashing = false;
        }

        protected virtual void Move() {
            if (!this.IsMoving) {
                return;
            }

            this.RootTransform.position += this.CurrentVelocity;
        }

        protected void FixedUpdate() {
            // Skip normal movement while dashing
            if (isDashing)
                return;

            this.CurrentVelocity = this.TargetDirection * (this.Speed * Time.fixedDeltaTime);
            this.Move();
        }
    }
}
