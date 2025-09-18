using SaintsField;
using UnityEngine;

namespace Game.AI {
    public sealed class VisualSensor2D : ProximitySensor2D {
        [field: SerializeField, MinValue(0)] private float DetectionHalfAngle { get; set; } = 180f;
        [field: SerializeField, Required] private Transform EyePosition { get; set; }
        [field: SerializeField, Required] private Transform LookAtPosition { get; set; }
        private Vector3 LookAtDirection { get; set; }

        protected override void Awake() {
            base.Awake();
            this.LookAtDirection = (this.LookAtPosition.position - this.EyePosition.position).normalized;
        }

        protected override bool IsValidTarget(GameObject target) {
            if (!base.IsValidTarget(target)) {
                return false;
            }
            
            Vector3 direction = target.transform.position - this.SelfTransform.position;
            float angle = Vector3.Angle(this.LookAtDirection, direction);
            return angle <= this.DetectionHalfAngle;
        }

        private void OnTriggerStay2D(Collider2D other) {
            if (this.IsValidTarget(other.gameObject)) {
                this.Capture(other.gameObject);
            } else {
                this.Release(other.gameObject);
            }
        }
    }
}
