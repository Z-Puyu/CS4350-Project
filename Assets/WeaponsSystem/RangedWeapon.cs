using System.Collections;
using System.Collections.Generic;
using Common;
using DataStructuresForUnity.Runtime.GeneralUtils;
using UnityEngine;
using WeaponsSystem.DamageHandling;
using WeaponsSystem.Projectiles;
using Timer = Utilities.Timer;

namespace WeaponsSystem {
    public class RangedWeapon : Weapon<RangedWeaponStats> {
        [field: SerializeField] private Projectile ProjectilePrefab { get; set; }
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
            } 
            
            OnScreenDebugger.Log("Cannot Attack, still in cooldown");
            return -1;
        }

        public override void DealDamage(
            Combatant combatant, ICollection<string> tags, LayerMask mask, Vector3 forward
        ) {
            if (!this.ProjectilePrefab) {
                return;
            }

            const int delay = 100;
            if (this.canAttack) {
                this.endTime = Time.time + this.Stats.GetCurrent(this.Stats.ShotsPerAttackAttribute) * delay / 1000.0f;
            }

            ProjectileConfig config = new ProjectileConfig(
                this.Stats.GetCurrent(this.Stats.ShotsPerAttackAttribute), delay, this.Stats.ProjectileMode, mask,
                tags, this.outwards, this.Stats.ProjectileEffects.ToArray()
            );
            
            Damage damage = new Damage(this.transform.root.gameObject, combatant, this.Stats.ReadDamageData());
            this.StartCoroutine(this.ProjectileSpawner.Spawn(this.ProjectilePrefab, this.Stats, config, damage, this.Hit));
            this.canAttack = false;
            this.fireIntervalTimer.Start();
        }
    }
}
