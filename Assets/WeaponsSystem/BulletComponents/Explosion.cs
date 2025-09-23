using Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines.ExtrusionShapes;

namespace WeaponsSystem.BulletComponents {
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
