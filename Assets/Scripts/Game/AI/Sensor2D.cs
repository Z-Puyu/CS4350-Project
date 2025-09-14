using UnityEngine;

namespace Game.AI {
    public abstract class Sensor2D : Sensor {
        protected void OnTriggerEnter2D(Collider2D other) {
            if (this.IsValidTarget(other.gameObject)) {
                this.Capture(other.gameObject);
            }
        }
        
        protected void OnTriggerExit2D(Collider2D other) {
            this.Release(other.gameObject);
        }
    }
}
