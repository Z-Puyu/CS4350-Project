using System;
using UnityEngine;
using UnityEngine.Events;

namespace InteractionSystem.Runtime {
    [DisallowMultipleComponent]
    public abstract class Interactable : MonoBehaviour {
        [field: SerializeField] private GameObject PromptWidget { get; set; }
        [field: SerializeField] private UnityEvent OnInteracted { get; set; } = new UnityEvent();
        private bool IsActive { get; set; }
        private Interactor Interactor { get; set; }

        private void OnDisable() {
            if (this.Interactor) {
                this.Interactor.Forget(this);
            }
            
            this.Sleep(this.Interactor);
            this.Interactor = null;
        }

        /// <summary>
        /// Checks if the interactor can interact with this object.
        /// </summary>
        /// <param name="interactor">The interactor.</param>
        /// <returns>True if the object can be interacted with.</returns>
        public abstract bool CanInteract(Interactor interactor);

        /// <summary>
        /// Triggers the interaction and broadcasts an event.
        /// </summary>
        /// <param name="interactor">The interactor.</param>
        public void TriggerInteraction(Interactor interactor) {
            this.OnInteract(interactor);
            this.OnInteracted.Invoke();
        }
        
        internal void Awaken(Interactor interactor) {
            if (this.IsActive) {
                return;
            }
            
            this.Interactor = interactor;
            this.OnAwaken(interactor);
            this.IsActive = true;
        }
        
        internal void Sleep(Interactor interactor) {
            if (!this.IsActive) {
                return;
            }
            
            this.Interactor = null;
            this.OnSleep(interactor);
            this.IsActive = false;
        }

        internal void Activate(Interactor interactor) {
            if (this.PromptWidget && interactor.CompareTag("Player")) {
                this.PromptWidget.SetActive(true);
            }
            
            this.OnActivate(interactor);
        }
        
        internal void Deactivate(Interactor interactor) {
            this.OnDeactivate(interactor);
            if (this.PromptWidget && interactor.CompareTag("Player")) {
                this.PromptWidget.SetActive(false);
            }
        }
        
        /// <summary>
        /// Behaviours when the interactor locks on this object as the target.
        /// </summary>
        /// <param name="interactor">The interactor.</param>
        protected virtual void OnActivate(Interactor interactor) { }
        
        /// <summary>
        /// Behaviours when the interactor switches away from this object to another target.
        /// </summary>
        /// <param name="interactor">The interactor.</param>
        protected virtual void OnDeactivate(Interactor interactor) { }

        /// <summary>
        /// Behaviours when the interactor interacts with this object.
        /// </summary>
        /// <param name="interactor">The interactor.</param>
        protected abstract void OnInteract(Interactor interactor);
        
        /// <summary>
        /// Behaviours when the interactor approaches this object.
        /// </summary>
        /// <param name="interactor">The interactor.</param>
        protected virtual void OnAwaken(Interactor interactor) { }
        
        /// <summary>
        /// Behaviours when the interactor leaves this object.
        /// </summary>
        /// <param name="interactor">The interactor.</param>
        protected virtual void OnSleep(Interactor interactor) { }
    }
}
