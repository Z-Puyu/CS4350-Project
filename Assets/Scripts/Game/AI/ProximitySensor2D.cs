using SaintsField;
using UnityEngine;

namespace Game.AI {
    [RequireComponent(typeof(CircleCollider2D))]
    public class ProximitySensor2D : Sensor2D {
        [field: SerializeField, MinValue(0)] private float DetectionRange { get; set; } = 10f;
        private CircleCollider2D Collider { get; set; }
        
        protected override void Awake() {
            base.Awake();
            this.Collider = this.GetComponent<CircleCollider2D>();
            this.Collider.radius = this.DetectionRange;
            this.Collider.isTrigger = true;
        }

        private void OnValidate() {
            this.GetComponent<CircleCollider2D>().isTrigger = true;
            this.GetComponent<CircleCollider2D>().radius = this.DetectionRange;
        }
    }
}
