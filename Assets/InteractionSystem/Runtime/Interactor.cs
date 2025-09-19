using System.Collections.Generic;
using System.Linq;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace InteractionSystem.Runtime {
	[DisallowMultipleComponent]
	public abstract class Interactor : MonoBehaviour {
		[field: SerializeField, MinValue(0)] protected float DetectionRadius { get; set; } = 3;
		private HashSet<Interactable> TargetsInRange { get; set; } = new HashSet<Interactable>();
		private Interactable Target { get; set; }
		[field: SerializeField] private Color GizmosColor { get; set; } = Color.yellow;

		public event UnityAction<Interactable> OnInteract; 
    
		private void OnDrawGizmosSelected() {
			Gizmos.color = this.GizmosColor;
			Gizmos.DrawWireSphere(this.transform.position, this.DetectionRadius);
		}

		/// <summary>
		/// Interacts with the target.
		/// </summary>
		public void Interact() { 
			if (!this.Target) {
				return;
			}

			this.OnInteract?.Invoke(this.Target);
			this.Target.TriggerInteraction(this);
		}
		
		internal void Approach(Interactable target) {
			this.TargetsInRange.Add(target);
			target.Awaken(this);
		}

		internal void Forget(Interactable target) {
			if (this.Target == target) {
				this.SwitchTarget(null);
			}
			
			if (this.TargetsInRange.Remove(target)) {
				target.Sleep(this);
			}
		}

		private void SwitchTarget(Interactable target) {
			if (this.Target == target) {
				return;
			}
			
			if (this.Target) {
				this.Target.Deactivate(this);
			}
			
			if (target) {
				target.Activate(this);
			}
			
			this.Target = target;
		}

		private protected void PollTarget() {
			this.TargetsInRange.RemoveWhere(obj => !obj);
			float minDistance = float.MaxValue;
			foreach (Interactable candidate in this.TargetsInRange) {
				candidate.Awaken(this);
				float dist = Vector2.SqrMagnitude(this.transform.position - candidate.transform.position);
				if (dist >= minDistance) {
					continue;
				}

				minDistance = dist;
				this.SwitchTarget(candidate);
			}
		}
	}
}
