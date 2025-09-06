using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Utilities;
using WeaponsSystem.WeaponComponent;

namespace WeaponsSystem {
    public abstract class Weapon : MonoBehaviour {
        [field: SerializeField] private WeaponData weaponData;
        [field: SerializeField] private List<WeaponComponentData> components;
        [field: SerializeField] private List<AttackData> attacks;
        
        
        public int CurrentAttackCounter {
            get => this.currentAttackCounter;
            private set {
                this.currentAttackCounter = value;
                this.currentAttackCounter = value >= this.weaponData.comboLength ?
                        0 : value;
            }
        }
        private int currentAttackCounter;
        private Timer comboResetTimer;
        public abstract void Attack();

        //place hold function. Should be adjusted after determining how to handle the attack.
        private void StartAttack() {
            this.comboResetTimer.Stop();
        }
        
        //place hold function. Should be adjusted after determining how to handle the attack.
        private void EndAttack() {
            this.CurrentAttackCounter += 1;
            this.comboResetTimer.Start();
            this.comboResetTimer.OnTimerFinished += this.ResetCombo;
        }

        private void Awake() {
            this.comboResetTimer = new Timer(this.weaponData.comboResetTime);
        }
        
        private void Update() {
            this.comboResetTimer.Tick();
        }
        
        private void ResetCombo() {
            this.CurrentAttackCounter = 0;
            this.comboResetTimer.OnTimerFinished -= this.ResetCombo;
        }
    }
}
