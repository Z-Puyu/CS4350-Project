using UnityEngine;

namespace Game.Map {
    [DisallowMultipleComponent]
    public sealed class Teleporter : MonoBehaviour {
        public void Activate() {
            this.GetComponent<SpriteRenderer>().color = Color.red;
        }
        
        public void Deactivate() {
            this.GetComponent<SpriteRenderer>().color = Color.white;
        }
        
        public void Boot() {
            this.GetComponent<SpriteRenderer>().color = Color.green;
        }
    }
}
