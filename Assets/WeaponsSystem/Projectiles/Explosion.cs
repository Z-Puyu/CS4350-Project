using Common;
using UnityEngine;

namespace WeaponsSystem.Projectiles {
    [RequireComponent(typeof(CircleCollider2D))]
    public class Explosion : MonoBehaviour {
        public void Explode() {
            this.GetComponent<CircleCollider2D>().enabled = true;
            OnScreenDebugger.Log("Exploded");
        }

        public void UpdateExplosionRadius(float radius) {
            this.GetComponent<CircleCollider2D>().radius = radius;
        }
    }
}
