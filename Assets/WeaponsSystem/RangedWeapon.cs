using Common;
using Unity.Cinemachine;
using UnityEngine;
using Utilities;

namespace WeaponsSystem {
    public class RangedWeapon : Weapon<RangedWeaponData>{
        [field: SerializeField] private ObjectPool<Bullet> bulletPool;
        private Timer fireIntervalTimer;
        private bool canAttack = true;
        
        public override void Attack() {

            if (this.canAttack) {
                OnScreenDebugger.Log("RangedAttackSuccessfully");
                this.StartAttack();
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

                    bullet.SetTarget(this.weaponData.BulletSpeed, this.weaponData.Range, direction, this.bulletPool);
                }
                this.EndAttack();
                this.canAttack = false;
                this.fireIntervalTimer.Start();
            } else {
                OnScreenDebugger.Log("Cannot Attack, still in cooldown");
            }
        }
        
        protected override void Awake() {
            base.Awake();
            this.bulletPool.Initialize(100);
            this.transform.position = this.transform.parent.position;
            this.fireIntervalTimer = new Timer(this.weaponData.FireInterval / 1000);
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
    }
}
