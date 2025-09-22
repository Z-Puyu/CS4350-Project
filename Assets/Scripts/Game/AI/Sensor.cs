using System.Collections.Generic;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace Game.AI {
    [DisallowMultipleComponent]
    public abstract class Sensor : MonoBehaviour {
        protected Transform SelfTransform { get; private set; }
        [field: SerializeField, Tag] private List<string> WatchedTags { get; set; } = new List<string>();
        [field: SerializeField] private UnityEvent<GameObject> OnDetected { get; set; } = new UnityEvent<GameObject>();
        [field: SerializeField] private UnityEvent<GameObject> OnLost { get; set; } = new UnityEvent<GameObject>();
        
        protected HashSet<GameObject> DetectedObjects { get; } = new HashSet<GameObject>();
        private Stack<GameObject> DetectedObjectStack { get; } = new Stack<GameObject>();
        public GameObject LastDetectedTarget => this.GetLastDetectedTarget();
        

        protected virtual void Awake() {
            this.SelfTransform = this.transform;
        }

        private void OnDestroy() {
            this.DetectedObjects.Clear();
        }

        private GameObject GetLastDetectedTarget() {
            while (this.DetectedObjectStack.TryPeek(out GameObject target)) {
                if (this.DetectedObjects.Contains(target)) {
                    return target;
                }
                
                this.DetectedObjectStack.Pop();
            }
            
            return null;
        }
        
        protected virtual bool IsValidTarget(GameObject target) {
            return this.WatchedTags.Count <= 0 || this.WatchedTags.Exists(target.CompareTag);
        }
        
        protected void Capture(GameObject target) {
            if (!this.DetectedObjects.Add(target)) {
                return;
            }

            this.DetectedObjectStack.Push(target);
            this.OnDetected.Invoke(target);
        }
        
        protected void Release(GameObject target) {
            if (this.DetectedObjects.Remove(target)) {
                this.OnLost.Invoke(target);
            }
        }
    }
}
