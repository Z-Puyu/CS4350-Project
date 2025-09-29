using System;
using UnityEngine;

namespace DataStructuresForUnity.Runtime.ObjectPooling {
    public abstract class PoolableObject : MonoBehaviour, IPoolable<PoolableObject> {
        public event Action<PoolableObject> OnReturn;
        private bool IsPooled { get; set; }
        
        public virtual void Initialise(Action<PoolableObject> onReturn) {
            if (this.IsPooled) {
#if DEBUG
                Debug.LogError("Object is already pooled", this.gameObject);
#endif
            }
            
            this.OnReturn += onReturn;
            this.IsPooled = true;
        }
        
        public virtual void Return() {
            this.OnReturn?.Invoke(this);
            this.OnReturn = null;
            this.IsPooled = false;
        }
    }
}
