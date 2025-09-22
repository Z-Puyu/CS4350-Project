using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.Events;
using Utilities;
using WeaponsSystem.BulletComponents;

namespace WeaponsSystem {
    public class RangedWeapon : Weapon<RangedWeaponStats> {
        [field: SerializeField] private ObjectPool<Bullet> bulletPool;
        private Timer fireIntervalTimer;
        private bool canAttack = true;

        protected override void Awake() {
            base.Awake();
            this.bulletPool.Initialize(100);
            this.transform.position = this.transform.parent.position;
            int fireInterval = this.Stats.GetCurrent(this.Stats.FireIntervalAttribute);
            this.fireIntervalTimer = new Timer(fireInterval / 1000.0f);
            this.fireIntervalTimer.OnTimerFinished += this.SetCanAttack;
        }

        protected override void Update() {
            base.Update();
            this.transform.position = this.transform.root.position;
            this.fireIntervalTimer.Tick();
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
            if (this.bulletPool != null) {
                Bullet bullet = this.bulletPool.GetInstance();
                bullet.transform.position = this.transform.position;
                bullet.gameObject.SetActive(true);
                Vector3 mousePosScreen = Input.mousePosition;
                Vector3 direction = Vector3.forward;
                if (Camera.main != null) {
                    Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePosScreen);
                    mousePosWorld.z = this.transform.position.z;
                    direction = (mousePosWorld - this.transform.position).normalized;
                }

                int speed = this.Stats.GetCurrent(this.Stats.BulletSpeedAttribute);
                int range = this.Stats.GetCurrent(this.Stats.RangeAttribute);
                bullet.SetTarget(speed, range, direction, this.bulletPool);
            }
            this.canAttack = false;
            this.fireIntervalTimer.Start();
        }
        
    }
}
