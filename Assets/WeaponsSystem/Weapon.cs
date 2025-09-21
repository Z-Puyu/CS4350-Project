using System;
using System.Collections.Generic;
using Common;
using NUnit.Framework;
using UnityEngine;
using Utilities;
using WeaponsSystem.WeaponComponents;

namespace WeaponsSystem {
    public struct AttackRecord {
    }

    public abstract class Weapon<D> : MonoBehaviour where D : WeaponData {
        [field: SerializeField] protected D weaponData;
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
        protected void StartAttack() {
            this.comboResetTimer.Stop();
            this.comboResetTimer.OnTimerFinished -= this.ResetCombo;
        }
        
        //place hold function. Should be adjusted after determining how to handle the attack.
        protected void EndAttack() {
            this.CurrentAttackCounter += 1;
            this.comboResetTimer.Start();
            this.comboResetTimer.OnTimerFinished += this.ResetCombo;
            OnScreenDebugger.Log($"AttackCounter {this.CurrentAttackCounter}");
            OnScreenDebugger.Log("End Attack");
        }

        
        

        protected virtual void Awake() {
            this.comboResetTimer = new Timer(this.weaponData.comboResetTime);
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
