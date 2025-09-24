using System.Collections.Generic;
using System.Linq;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;
using Utilities;
using WeaponsSystem.DamageHandling;

namespace WeaponsSystem.Projectiles {
    [RequireComponent(typeof(Explosion), typeof(Piercer))]
    public class Projectile : MonoBehaviour {
        public Vector3 speed;
        public float range;
        private float distanceTravelled;
        private ObjectPool<Projectile> pool;
        private UnityAction onHit;
        private Damage damageRecord;
        [SerializeField, Tag] private List<string> tags = new List<string>();

        public void SetTarget(float spd, float rg, Vector3 dir, ObjectPool<Projectile> bulletPool) {
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

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.CompareTag("Player")) {
                return;
            }

            if (other.TryGetComponent(out IDamageable damageable)) {
                this.DealDamage(other, this.tags);
                this.onHit?.Invoke();
            }
        }

        public void Awake() {
            this.onHit += this.GetComponent<Explosion>().Explode;
            this.onHit += this.GetComponent<Piercer>().Hit;
        }
        
        public void SetDamage(Damage damage) {
            this.damageRecord = damage;
        }

        private void DealDamage(Collider2D other, List<string> tags) {
            if (tags.Any(other.gameObject.CompareTag)) {
                other.TryGetComponent(out IDamageable damageable);
                damageable.HandleDamage(this.damageRecord);
            }
        }
    }
}
