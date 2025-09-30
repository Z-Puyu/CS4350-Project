using System;
using System.Collections.Generic;
using DataStructuresForUnity.Runtime.Utilities;
using UnityEngine;

namespace Weapons.Runtime {
    public abstract class Weapon : MonoBehaviour, IWeapon {
        [field: SerializeReference]
        private List<IWeaponController> WeaponControllers { get; set; } = new List<IWeaponController>();
        
        [field: SerializeReference] protected List<AttackStrategy> Combos { get; } = new List<AttackStrategy>();
        protected Dictionary<Type, IWeaponController> Controllers { get; } = new Dictionary<Type, IWeaponController>();
        public WeaponComboController ComboController { get; set; }
        private WeaponAttackController AttackController { get; set; }
        protected WeaponStats Stats { get; set; }
        private float ControllerProcessingTime { get; set; }
        public float WaitingTimeUntilNextAttack => Time.time + this.ControllerProcessingTime;
        
        private Timer ComboResetTimer { get; set; }

        private void Awake() {
            foreach (IWeaponController controller in this.WeaponControllers) {
                this.Controllers.Add(controller.GetType(), controller);
            }

            foreach (IWeaponController controller in this.WeaponControllers) {
                controller.Possess(this, this.Stats);
            }
            
            this.ComboController = this.GetController<WeaponComboController>();
            this.AttackController = this.GetController<WeaponAttackController>();
            this.ComboResetTimer = new Timer(this.ComboController.ComboResetTime);
        }

        public C GetController<C>() where C : IWeaponController, new() {
            if (this.Controllers.TryGetValue(typeof(C), out IWeaponController controller)) {
                return (C)controller;
            }
            
            C newController = new C();
            this.Controllers.Add(typeof(C), newController);
            return newController;
        }
        
        public void StartAttack() {
            this.ComboResetTimer.Stop();
            this.ComboResetTimer.OnTimerFinished -= this.OnComboTimeOut;
        }

        public virtual void PerformAttack(
            List<string> attackableTags, LayerMask attackableLayers, Vector3 attackPosition, Vector3 attackDirection
        ) {
            int index = this.ComboController.CurrentComboIndex;
            AttackAction action = new AttackAction(
                attackableTags, attackableLayers, attackPosition, attackDirection, index
            );

            float cooldown = 0;
            foreach (IWeaponController controller in this.Controllers.Values) {
                cooldown = Mathf.Max(cooldown, controller.UpdateOnAttack(action));
            }
            
            this.ControllerProcessingTime = cooldown;
            this.ComboResetTimer = new Timer(this.ComboController.ComboResetTime + cooldown);
            this.ComboResetTimer.OnTimerFinished += this.OnComboTimeOut;
            this.ComboResetTimer.Start();
        }
        
        private void OnComboTimeOut() {
            this.ComboController.CurrentComboIndex = 0;
            this.ComboResetTimer.Stop();
            this.ComboResetTimer.OnTimerFinished -= this.OnComboTimeOut;
        }

        public virtual void EndAttack() {
            
        }

        private void LateUpdate() {
            this.ComboResetTimer.Tick();
        }
    }
}
