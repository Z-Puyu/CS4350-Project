using Common;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using Utilities;

namespace WeaponsSystem {
    public class RangedWeapon : Weapon {
        [field: SerializeField] private ObjectPool<Bullet> bulletPool;
        private Timer fireIntervalTimer;
        private bool canAttack = true;
        
        [field: SerializeField, Layout("WeaponStats", ELayout.Foldout)] 
        private bool UseAttributeForBulletSpeed { get; set; }
        
        [field: SerializeField, ShowIf(nameof(this.UseAttributeForBulletSpeed))] 
        private AttributeTypeDefinition BulletSpeedAttribute { get; set; }

        [field: SerializeField, HideIf(nameof(this.UseAttributeForBulletSpeed))]
        [field: RichLabel("Bullet Speed"), MinValue(0)]
        private float DefaultBulletSpeed { get; set; }
        
        [field: SerializeField] private bool UseAttributeForRange { get; set; }
        
        [field: SerializeField, ShowIf(nameof(this.UseAttributeForRange))] 
        private AttributeTypeDefinition RangeAttribute { get; set; }
        
        [field: SerializeField, HideIf(nameof(this.UseAttributeForRange)), RichLabel("Range"), MinValue(0)]
        private float DefaultRange { get; set; }
        
        [field: SerializeField] private bool UseAttributeForSpread { get; set; }
        
        [field: SerializeField, ShowIf(nameof(this.UseAttributeForSpread))] 
        private AttributeTypeDefinition SpreadAttribute { get; set; }
        
        [field: SerializeField, HideIf(nameof(this.UseAttributeForSpread)), RichLabel("Spread"), MinValue(0)]
        private float DefaultSpread { get; set; }
        
        [field: SerializeField] private bool UseAttributeForFireInterval { get; set; }
        
        [field: SerializeField, ShowIf(nameof(this.UseAttributeForFireInterval))]
        private AttributeTypeDefinition FireIntervalAttribute { get; set; }
        
        [field: SerializeField, HideIf(nameof(this.UseAttributeForFireInterval))]
        [field: RichLabel("Fire Interval"), MinValue(0)]
        private float DefaultFireInterval { get; set; }

        private float BulletSpeed => this.BulletSpeedAttribute
                ? this.AttributeSet.GetCurrent(this.BulletSpeedAttribute.Id)
                : this.DefaultBulletSpeed;
        
        private float Range => this.RangeAttribute
                ? this.AttributeSet.GetCurrent(this.RangeAttribute.Id)
                : this.DefaultRange;
        
        private float Spread => this.SpreadAttribute
                ? this.AttributeSet.GetCurrent(this.SpreadAttribute.Id)
                : this.DefaultSpread;
        
        private float FireInterval => this.FireIntervalAttribute
                ? this.AttributeSet.GetCurrent(this.FireIntervalAttribute.Id)
                : this.DefaultFireInterval;
        

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

                    bullet.SetTarget(this.BulletSpeed, this.Range, direction, this.bulletPool);
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
            this.fireIntervalTimer = new Timer(this.FireInterval / 1000);
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
