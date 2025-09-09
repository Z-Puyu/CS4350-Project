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

        private void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent(out Interactable interactable)) {
                this.TargetsInRange.Add(interactable);
            }
        }

        private void OnTriggerExit(Collider other) {
            if (!other.TryGetComponent(out Interactable interactable)) {
                return;
            }

            this.TargetsInRange.Remove(interactable);
            interactable.Deactivate(this);
        }

        private void Update() { 
            this.PollTarget();
        }
    }
}
