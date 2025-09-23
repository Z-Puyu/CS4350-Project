using System.Collections.Generic;
using Common;
using SaintsField;
using UnityEngine;
using Utilities;
using WeaponsSystem.WeaponComponent;

namespace WeaponsSystem {
    public abstract class Weapon<S> : MonoBehaviour, IDamageDealer where S : WeaponStats {
        [field: SerializeField] protected WeaponData WeaponData { get; private set; }
        [field: SerializeField] private List<WeaponComponentData> Components { get; set; }
        [field: SerializeField, Required] protected S Stats { get; private set; }

        public int CurrentAttackCounter {
            get => this.currentAttackCounter;
            private set => this.currentAttackCounter =
                    value % this.Stats.GetCurrent(this.Stats.ComboLengthAttribute);
        }

        private int currentAttackCounter;
        private Timer comboResetTimer;
        
        protected virtual void Awake() {
            if (!this.Stats) {
                this.Stats = this.GetComponentInChildren<S>();
            }
            
            this.comboResetTimer = new Timer(this.WeaponData.ComboResetTime);
        }

        private void Start() {
            this.Stats.Initialise(this.WeaponData);
        }
        
        public virtual bool AllowsDamageOn(GameObject candidate) {
            return true;
        }

        //place hold function. Should be adjusted after determining how to handle the attack.
        public virtual int StartAttack() {
            this.Stats.ActivateAttackModifiers(this.CurrentAttackCounter);
            this.comboResetTimer.Stop();
            this.comboResetTimer.OnTimerFinished -= this.ResetCombo;
            return this.CurrentAttackCounter;
        }
        
        public abstract void DealDamage(ICollection<string> tags, LayerMask mask, Vector3 forward);
        
        //place hold function. Should be adjusted after determining how to handle the attack.
        public virtual void EndAttack() {
            this.Stats.DeactivateAttackModifiers(this.CurrentAttackCounter);
            this.CurrentAttackCounter += 1;
            this.comboResetTimer.Start();
            this.comboResetTimer.OnTimerFinished += this.ResetCombo;
        }

        protected virtual void Update() {
            this.comboResetTimer.Tick();
        }

        private void ResetCombo() {
            this.CurrentAttackCounter = 0;
            this.comboResetTimer.OnTimerFinished -= this.ResetCombo;
            OnScreenDebugger.Log("Combo Reset");
        }
    }
}
