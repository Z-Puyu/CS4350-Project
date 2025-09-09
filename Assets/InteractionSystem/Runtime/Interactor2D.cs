using System;
using System.Linq;
using SaintsField;
using UnityEngine;

namespace InteractionSystem.Runtime {
    [DisallowMultipleComponent]
    public sealed class Interactor2D : Interactor {
        private void Awake() {
            GameObject trigger = new GameObject("Interactor's Collider");
            trigger.transform.SetParent(this.transform);
            trigger.transform.localPosition = Vector3.zero;
            CircleCollider2D circle = trigger.AddComponent<CircleCollider2D>();
            circle.radius = this.DetectionRadius;
            circle.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.TryGetComponent(out Interactable interactable)) {
                this.Approach(interactable);
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (other.TryGetComponent(out Interactable interactable)) {
                this.Forget(interactable);
            }
        }

        private void Update() { 
            this.PollTarget();
        }
    }
}
