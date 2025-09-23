using System;
using System.Collections.Generic;
using Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Utilities;
using WeaponsSystem.BulletComponents;
using WeaponsSystem.DamageHandling;
using Timer = Utilities.Timer;

namespace WeaponsSystem {
    public enum ProjectileSpawnMethod {
        Spread,
        Parallel,
        Multitap
    }

    public class RangedWeapon : Weapon<RangedWeaponStats> {
        [field: SerializeField] private ObjectPool<Bullet> bulletPool;
        private Timer fireIntervalTimer;
        private bool canAttack = true;
        private Vector3 outwards;
        private Camera mainCamera;
        private List<ProjectileSpawnMethod> spawnMethods;

        protected override void Awake() {
            base.Awake();
            this.bulletPool.Initialize(100);
            this.transform.position = this.transform.parent.position;
            int fireInterval = this.Stats.GetCurrent(this.Stats.FireIntervalAttribute);
            this.fireIntervalTimer = new Timer(fireInterval / 1000.0f);
            this.fireIntervalTimer.OnTimerFinished += this.SetCanAttack;
            this.mainCamera = Camera.main;
        }

        protected override void Update() {
            base.Update();
            this.fireIntervalTimer.Tick();
            Vector3 mousePosScreen = Input.mousePosition;
            Vector3 direction = Vector3.forward;
            if (this.mainCamera != null) {
                Vector3 mousePosWorld = this.mainCamera.ScreenToWorldPoint(mousePosScreen);
                mousePosWorld.z = this.transform.position.z;
                this.outwards = (mousePosWorld - this.transform.position).normalized;
            }
        }

        private void SetCanAttack() {
            this.canAttack = true;
        }
        
        public override int StartAttack() {
            if (this.canAttack) {
                OnScreenDebugger.Log("RangedAttackSuccessfully");
                return base.StartAttack();
            } else {
                OnScreenDebugger.Log("Cannot Attack, still in cooldown");
            }
            return -1;
        }

        public override void DealDamage(ICollection<string> tags, LayerMask mask, Vector3 forward) {
            if (this.bulletPool == null) {
                return;
            }

            this.StartCoroutine(this.SpawnMultitapBullet(this.Stats.GetCurrent(this.Stats.MultitapCountAttribute), 100));
            this.canAttack = false;
            this.fireIntervalTimer.Start();
        }

        private void SpawnSingleBullet(Vector3 direction, Vector3 position, int speed, int range) {
            if (this.bulletPool == null) {
                return;
            }

            Bullet bullet = this.bulletPool.GetInstance();
            bullet.transform.position = position;
            bullet.gameObject.SetActive(true);
            bullet.SetTarget(speed, range, direction, this.bulletPool);
            bullet.SetDamage(new Damage(this.transform.root.gameObject, this.Stats.ReadDamageData()));
        }

        private void SpawnSpreadBullet(Vector3 direction, int spread, int count) {
            float startAngle = -spread / 2.0f;
            float angleStep = spread / (count - 1.0f);
            for (int i = 0; i < count; i += 1) {
                float currentAngle = startAngle + i * angleStep;
                Vector3 currentDirection = Quaternion.Euler(0, 0, currentAngle) * direction;
                this.SpawnSingleBullet(currentDirection, this.transform.position, this.Stats.GetCurrent(this.Stats.BulletSpeedAttribute), this.Stats.GetCurrent(this.Stats.RangeAttribute));
            }
        }
        
        private void SpawnParallelBullet(Vector3 direction, float spacing, int count) {
            Vector3 orthogonal = Vector3.Cross(direction, Vector3.forward).normalized;
            float interval = spacing / (count - 1.0f);
            float startOffset = -(spacing / 2.0f);
            for (int i = 0; i < count; i += 1) {
                Debug.Log("SpawnParallelBullet: " + (startOffset + interval * i) * orthogonal);
                this.SpawnSingleBullet(direction, this.transform.position + (startOffset + interval * i) * orthogonal, this.Stats.GetCurrent(this.Stats.BulletSpeedAttribute), this.Stats.GetCurrent(this.Stats.RangeAttribute));
            }
        }
        
        private System.Collections.IEnumerator SpawnMultitapBullet(int count, int delay) {
            for (int i = 0; i < count; i += 1) {
                this.SpawnSingleBullet(this.outwards, this.transform.position, this.Stats.GetCurrent(this.Stats.BulletSpeedAttribute), this.Stats.GetCurrent(this.Stats.RangeAttribute));
                yield return new WaitForSeconds(delay / 1000.0f);
            }
        }
    }
}
