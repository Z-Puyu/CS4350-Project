using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace DataStructuresForUnity.Runtime.GeneralUtils {
    [DisallowMultipleComponent]
    public class PoolableObject : MonoBehaviour {
        [field: SerializeField] public string PoolableId { get; private set; }
        public event Action OnReturned;

        public virtual void Return() {
            this.OnReturned?.Invoke();
            this.OnReturned = null;
        }
    }
}
