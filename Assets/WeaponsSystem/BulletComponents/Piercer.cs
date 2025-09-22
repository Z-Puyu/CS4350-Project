using Common;
using UnityEngine;

namespace WeaponsSystem.BulletComponents {
    public class Piercer : MonoBehaviour {
        public int PierceCount { get; private set; }

        public void Hit() {
            this.PierceCount -= 1;
            OnScreenDebugger.Log($"Pierced {this.PierceCount} times");
            if (this.PierceCount <= 0) {
                this.gameObject.SetActive(false);
            }
        }
    }
}
