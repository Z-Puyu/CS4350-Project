using System;
using UnityEngine;
using UnityEngine.Events;

namespace InteractionSystem.Runtime {
    [DisallowMultipleComponent]
    public class Interactable : MonoBehaviour {
        [field: SerializeField] private GameObject PromptWidget { get; set; }

        [field: SerializeField]
        private UnityEvent<Interactor> OnInteracted { get; set; } = new UnityEvent<Interactor>();
        
        [field: SerializeField]
        private UnityEvent<Interactor> OnAwakened { get; set; } = new UnityEvent<Interactor>();
        
        [field: SerializeField]
        private UnityEvent<Interactor> OnSleep { get; set; } = new UnityEvent<Interactor>();
        
        [field: SerializeField]
        private UnityEvent<Interactor> OnActivate { get; set; } = new UnityEvent<Interactor>();
        
        [field: SerializeField]
        private UnityEvent<Interactor> OnDeactivate { get; set; } = new UnityEvent<Interactor>();
        
        private bool IsActive { get; set; }
        private Interactor Interactor { get; set; }

        void Start() {
            if (this.PromptWidget) {
                this.PromptWidget.SetActive(false);
            }
        }

        private void OnDisable() {
            if (this.Interactor) {
                this.Interactor.Forget(this);
            }
            
            this.Sleep(this.Interactor);
            this.Interactor = null;
        }

        /// <summary>
        /// Triggers the interaction and broadcasts an event.
        /// </summary>
        /// <param name="interactor">The interactor.</param>
        internal void TriggerInteraction(Interactor interactor) {
            this.OnInteracted.Invoke(interactor);
        }
        
        internal void Awaken(Interactor interactor) {
            if (this.IsActive) {
                return;
            }
            
            this.Interactor = interactor;
            this.OnAwakened.Invoke(interactor);
            this.IsActive = true;
        }
        
        internal void Sleep(Interactor interactor) {
            if (!this.IsActive) {
                return;
            }
            
            this.Interactor = null;
            this.OnSleep.Invoke(interactor);
            this.IsActive = false;
        }

        internal void Activate(Interactor interactor) {
            if (this.PromptWidget && interactor.CompareTag("Player")) {
                this.PromptWidget.SetActive(true);
            }
            
            this.OnActivate.Invoke(interactor);
        }
        
        internal void Deactivate(Interactor interactor) {
            this.OnDeactivate.Invoke(interactor);
            if (this.PromptWidget && interactor.CompareTag("Player")) {
                this.PromptWidget.SetActive(false);
            }
        }
    }
}
