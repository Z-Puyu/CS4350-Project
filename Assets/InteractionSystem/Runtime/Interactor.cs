using System.Collections.Generic;
using System.Linq;
using SaintsField;
using UnityEngine;

namespace InteractionSystem.Runtime {
	[DisallowMultipleComponent]
	public abstract class Interactor : MonoBehaviour {
		[field: SerializeField, MinValue(0)] protected float DetectionRadius { get; set; } = 3;
		private protected HashSet<Interactable> TargetsInRange { get; private set; } = new HashSet<Interactable>();
		private protected Interactable Target { get; private set; }
    

		[field: SerializeField, Header("Gizmos")]
		private Color GizmosColor { get; set; } = Color.yellow;
    
		private void OnDrawGizmosSelected() {
			Gizmos.color = this.GizmosColor;
			Gizmos.DrawWireSphere(this.transform.position, this.DetectionRadius);
		}

		internal void Forget(Interactable target) {
			this.TargetsInRange.Remove(target);
			if (this.Target == target) {
				this.SwitchTarget(null);
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
			float minDistance = float.MaxValue;
			foreach (Interactable candidate in this.TargetsInRange.Where(obj => obj.CanInteract(this))) {
				candidate.Activate(this);
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
