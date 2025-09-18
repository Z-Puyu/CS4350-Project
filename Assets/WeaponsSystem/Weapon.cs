using System.Collections.Generic;
using Common;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using Utilities;
using WeaponsSystem.DamageHandling;
using WeaponsSystem.WeaponComponent;

namespace WeaponsSystem {
    [RequireComponent(typeof(AttributeSet))]
    public abstract class Weapon : MonoBehaviour {
        [field: SerializeField] protected WeaponData WeaponData { get; private set; }
        [field: SerializeField] private List<WeaponComponentData> Components { get; set; }
        [SerializeField] private List<AttackData> attacks;
        
        [field: SerializeField, Layout("WeaponStats", ELayout.Foldout)] 
        protected bool UseAttributeForComboLength { get; private set; }
        
        [field: SerializeField, ShowIf(nameof(this.UseAttributeForComboLength))] 
        protected AttributeTypeDefinition ComboLengthAttribute { get; private set; }

        [field: SerializeField, HideIf(nameof(this.UseAttributeForComboLength))]
        [field: RichLabel("Bullet Speed"), MinValue(0)]
        protected float DefaultComboLength { get; private set; }

        protected AttributeSet AttributeSet { get; private set; }

        public int CurrentAttackCounter {
            get => this.currentAttackCounter;
            private set => this.currentAttackCounter =
                    value % this.AttributeSet.GetCurrent(this.ComboLengthAttribute.Id);
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
            this.AttributeSet = this.GetComponent<AttributeSet>();
            this.comboResetTimer = new Timer(this.WeaponData.ComboResetTime);
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
