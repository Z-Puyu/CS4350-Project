using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utilities;
using WeaponsSystem.DamageHandling;

namespace WeaponsSystem.BulletComponents {
    [RequireComponent(typeof(Explosion), typeof(Piercer))]
    public class Bullet : MonoBehaviour {
        public Vector3 speed;
        public float range;
        private float distanceTravelled;
        private ObjectPool<Bullet> pool;
        private UnityAction onHit;

        public void SetTarget(float spd, float rg, Vector3 dir, ObjectPool<Bullet> bulletPool) {
            this.speed = spd * dir;
            this.range = rg;
            this.pool = bulletPool;
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

        private void OnCollisionEnter2(Collision2D collision) {
            if (collision.gameObject.CompareTag("Player")) {
                return;
            }

            this.DealDamage(collision);
            this.onHit?.Invoke();
        }

        public void Awake() {
            this.onHit += this.GetComponent<Explosion>().Explode;
            this.onHit += this.GetComponent<Piercer>().Hit;
        }

        private void DealDamage(Collision2D other) {
            if (other.gameObject.CompareTag("Enemy")) {
                
            }
        }
    }
}
