using System.Collections;
using System.Collections.Generic;
using Common;
using GameplayAbilities.Runtime.Attributes;
using UnityEngine;
using Utilities;
using WeaponsSystem.DamageHandling;
using WeaponsSystem.Projectiles;
using Timer = Utilities.Timer;

namespace WeaponsSystem {
    public class RangedWeapon : Weapon<RangedWeaponStats> {
        public enum ProjectileSpawnMethod {
            Spread,
            Parallel,
            Single,
            Multitap
        }

        [field: SerializeField] private ObjectPool<Projectile> ProjectilePool { get; set; }
        [field: SerializeField] private List<ProjectileSpawnMethod> spawnMethods;
        private Timer fireIntervalTimer;
        private bool canAttack = true;
        private Vector3 outwards;
        private Camera mainCamera;
        private float endTime;
        private Transform SelfTransform { get; set; }
        public override float AttackDuration => this.endTime - Time.time;


        protected override void Awake() {
            base.Awake();
            this.SelfTransform = this.transform;
            this.ProjectilePool.Initialize(100);
            this.mainCamera = Camera.main;
        }

        protected override void Start() {
            base.Start();
            this.SelfTransform.position = this.SelfTransform.parent.position;
            int fireInterval = this.Stats.GetCurrent(this.Stats.FireIntervalAttribute);
            this.fireIntervalTimer = new Timer(fireInterval / 1000.0f);
            this.fireIntervalTimer.OnTimerFinished += () => this.canAttack = true;
        }

        protected override void Update() {
            base.Update();
            this.fireIntervalTimer.Tick();
            Vector3 mousePosScreen = Input.mousePosition;
            if (!this.mainCamera) {
                return;
            }

            Vector3 mousePosWorld = this.mainCamera.ScreenToWorldPoint(mousePosScreen);
            Vector3 position = this.SelfTransform.position;
            mousePosWorld.z = position.z;
            this.outwards = (mousePosWorld - position).normalized;
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
            if (this.ProjectilePool is null) {
                return;
            }

            const int delay = 100;
            if (this.canAttack) {
                this.endTime = Time.time + this.Stats.GetCurrent(this.Stats.MultitapCountAttribute) * delay / 1000.0f;
            }

            this.StartCoroutine(
                this.SpawnMultitapBullet(
                    this.Stats.GetCurrent(this.Stats.MultitapCountAttribute), delay,
                    this.spawnMethods[this.CurrentAttackCounter], mask
                )
            );
            this.canAttack = false;
            this.fireIntervalTimer.Start();
        }

        private void SpawnSingleBullet(Vector3 direction, Vector3 position, LayerMask mask) {
            if (this.ProjectilePool is null) {
                return;
            }

            Damage damage = new Damage(this.transform.root.gameObject, this.Stats.ReadDamageData());
            this.ProjectilePool
                .GetInstance(position)
                .WithDamage(damage)
                .OnHit(this.Hit)
                .Launch(this.Stats, direction, this.ProjectilePool, mask);
        }

        private void SpawnSpreadBullet(Vector3 direction, int spread, int count, LayerMask mask) {
            float startAngle = -spread / 2.0f;
            float angleStep = spread / (count - 1.0f);
            for (int i = 0; i < count; i += 1) {
                float currentAngle = startAngle + i * angleStep;
                Vector3 currentDirection = Quaternion.Euler(0, 0, currentAngle) * direction;
                this.SpawnSingleBullet(currentDirection, this.transform.position, mask);
            }
        }

        private void SpawnParallelBullet(Vector3 direction, float spacing, int count, LayerMask mask) {
            Vector3 orthogonal = Vector3.Cross(direction, Vector3.forward).normalized;
            float interval = spacing / (count - 1.0f);
            float startOffset = -(spacing / 2.0f);
            for (int i = 0; i < count; i += 1) {
                Vector3 position = this.transform.position + (startOffset + interval * i) * orthogonal;
                this.SpawnSingleBullet(direction, position, mask);
            }
        }

        private IEnumerator SpawnMultitapBullet(int count, int delay, ProjectileSpawnMethod spawnMethod, LayerMask mask) {
            for (int i = 0; i < count; i += 1) {
                switch (spawnMethod) {
                    case ProjectileSpawnMethod.Spread:
                        this.SpawnSpreadBullet(
                            this.outwards, this.Stats.GetCurrent(this.Stats.ProjectileSpreadAttribute),
                            this.Stats.GetCurrent(this.Stats.ProjectileCountAttribute), mask
                        );
                        break;
                    case ProjectileSpawnMethod.Parallel:
                        this.SpawnParallelBullet(
                            this.outwards, this.Stats.GetCurrent(this.Stats.FireSpacingAttribute),
                            this.Stats.GetCurrent(this.Stats.ProjectileCountAttribute), mask
                        );
                        break;
                    case ProjectileSpawnMethod.Single or ProjectileSpawnMethod.Multitap:
                    default:
                        this.SpawnSingleBullet(this.outwards, this.transform.position, mask);
                        break;
                }

                yield return new WaitForSeconds(delay / 1000.0f);
            }
        }
    }
}
