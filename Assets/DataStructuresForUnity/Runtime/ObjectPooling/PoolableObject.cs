using System;
using UnityEngine;

namespace DataStructuresForUnity.Runtime.ObjectPooling {
    public abstract class PoolableObject<T> : MonoBehaviour, IPoolable<T> where T : PoolableObject<T> {
        public event Action<T> OnReturn;
        private bool IsPooled { get; set; }
        
        public virtual void Initialise(Action<T> onReturn) {
            if (this.IsPooled) {
#if DEBUG
                Debug.LogError("Object is already pooled", this.gameObject);
#endif
            }
            
            this.OnReturn += onReturn;
            this.IsPooled = true;
        }
        
        public virtual void Return() {
            this.OnReturn?.Invoke((T)this);
            this.OnReturn = null;
            this.IsPooled = false;
        }
    }
}
