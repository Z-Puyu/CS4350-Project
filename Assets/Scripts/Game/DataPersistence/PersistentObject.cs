using UnityEngine;

namespace Game.DataPersistence {
    [DisallowMultipleComponent]
    public sealed class PersistentObject : MonoBehaviour {
        private void Awake() {
            Object.DontDestroyOnLoad(this.gameObject);
        }
    }
}
