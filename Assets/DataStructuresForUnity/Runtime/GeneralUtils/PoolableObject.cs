using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace DataStructuresForUnity.Runtime.GeneralUtils {
    [DisallowMultipleComponent]
    public abstract class PoolableObject : MonoBehaviour {
        public abstract string PoolableId { get; }
        public event Action OnReturned;

        public virtual void Return() {
            this.OnReturned?.Invoke();
            this.OnReturned = null;
        }
    }
}
