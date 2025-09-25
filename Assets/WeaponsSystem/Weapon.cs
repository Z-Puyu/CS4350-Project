using System.Collections.Generic;
using Common;
using DataStructuresForUnity.Runtime.GeneralUtils;
using SaintsField;
using UnityEngine;
using Utilities;
using WeaponsSystem.WeaponComponent;

namespace WeaponsSystem {
    [RequireComponent(typeof(ComponentManager))]
    public abstract class Weapon<S> : MonoBehaviour, IDamageDealer where S : WeaponStats {
        [field: SerializeField] protected WeaponData WeaponData { get; private set; }
        [field: SerializeField, Required] protected S Stats { get; private set; }
        
        // Placeholder 
        [field: SerializeField] private ParticleSystem ParticleEffect { get; set; }
        
        public abstract float AttackDuration { get; }
        private int currentAttackCounter;

        protected int CurrentAttackCounter {
            get => this.currentAttackCounter;
            private set => this.currentAttackCounter =
                    value % this.Stats.GetCurrent(this.Stats.ComboLengthAttribute);
        }

        
        private Timer ComboResetTimer { get; set; }
        
        protected virtual void Awake() {
            if (!this.Stats) {
                this.Stats = this.GetComponentInChildren<S>();
            }
            
            this.ComboResetTimer = new Timer(this.WeaponData.ComboResetTime);
        }

        protected virtual void Start() {
            this.Stats.Initialise(this.WeaponData);
        }
        
        public virtual bool AllowsDamageOn(GameObject candidate) {
            return true;
        }

        public void Enable() {
            this.gameObject.SetActive(true);
        }
        
        public void Disable() {
            this.CurrentAttackCounter = 0;
            this.gameObject.SetActive(false);
        }

        public virtual int StartAttack() {
            this.Stats.ActivateAttackModifiers(this.CurrentAttackCounter);
            this.ComboResetTimer.Stop();
            this.ComboResetTimer.OnTimerFinished -= this.ResetCombo;
            return this.CurrentAttackCounter;
        }

        protected void Hit(Vector3 at) {
            ObjectSpawner.Spawn(this.ParticleEffect, at, Quaternion.identity);
        }

        public abstract void DealDamage(ICollection<string> tags, LayerMask mask, Vector3 forward);
        
        public virtual void EndAttack() {
            this.Stats.DeactivateAttackModifiers(this.CurrentAttackCounter);
            this.CurrentAttackCounter += 1;
            this.ComboResetTimer.Start();
            this.ComboResetTimer.OnTimerFinished += this.ResetCombo;
        }
        
        private void ResetCombo() {
            this.CurrentAttackCounter = 0;
            this.ComboResetTimer.OnTimerFinished -= this.ResetCombo;
        }

        protected virtual void Update() {
            this.ComboResetTimer.Tick();
        }
    }
}
