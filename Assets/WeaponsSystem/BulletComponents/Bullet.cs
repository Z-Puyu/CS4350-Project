using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

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

        private void OnCollisionEnter2D(Collision2D collision) {
            this.onHit?.Invoke();
        }

        public void Awake() {
            this.onHit += this.GetComponent<Explosion>().Explode;
            this.onHit += this.GetComponent<Piercer>().Hit;
        }

        public void DealDamage(ICollection<string> tags, LayerMask mask, Vector3 forward) {
            
        }
    }
}
