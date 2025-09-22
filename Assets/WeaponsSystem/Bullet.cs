using Common;
using UnityEngine;
using Utilities;

namespace WeaponsSystem {
    public class Bullet : MonoBehaviour {
        public Vector3 speed;
        public float range;
        private float distanceTravelled = 0;
        private ObjectPool<Bullet> pool;
        
        public Bullet() { }

        public void SetTarget(float spd, float rg, Vector3 dir, ObjectPool<Bullet> pool) {
            this.speed = spd * dir;
            this.range = rg;
            this.pool = pool;
        }

        private void Update() {
            if (this.distanceTravelled >= this.range) {
                this.distanceTravelled = 0;
                this.pool.ReturnInstance(this);
                return;
            }
            Vector3 distanceTravelledThisFrame = Time.deltaTime * this.speed;
            this.distanceTravelled += distanceTravelledThisFrame.magnitude;
            this.transform.position += distanceTravelledThisFrame;
        }
        
        private void OnCollisionEnter2D(Collision2D collision) {
            if (collision.gameObject.CompareTag("Damageable")) {
                OnScreenDebugger.Log($"Hit! Target is {collision.gameObject.name}");
                this.pool.ReturnInstance(this);
            }

            this.distanceTravelled = 0;
            
            // TODO: Handle collision, need implementation of enemy.
        }
    }
}
