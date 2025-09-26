using System.Collections.Generic;
using DataStructuresForUnity.Runtime.Bitmasks;
using DataStructuresForUnity.Runtime.GeneralUtils;
using SaintsField;
using UnityEngine;
using Utilities;
using WeaponsSystem.DamageHandling;
using WeaponsSystem.Projectiles;
using WeaponsSystem.WeaponComponent;

namespace WeaponsSystem {
    [RequireComponent(typeof(ProjectileSpawner))]
    public abstract class Weapon<S> : MonoBehaviour, IDamageDealer where S : WeaponStats {
        [field: SerializeField] private ComponentSet PossibleComponents { get; set; }
        [field: SerializeField] protected WeaponData WeaponData { get; private set; }
        [field: SerializeField, Required] protected S Stats { get; private set; }
        [field: SerializeField, Required] private ComponentManager ComponentManager { get; set; }
        protected ProjectileSpawner ProjectileSpawner { get; private set; }
        
        public Bitmask64 ComponentCombination => this.ComponentManager.ComponentCombination;
        
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
            
            this.ProjectileSpawner = this.GetComponent<ProjectileSpawner>();
            this.ComboResetTimer = new Timer(this.WeaponData.ComboResetTime);
        }

        protected virtual void Start() {
            this.Stats.Initialise(this.WeaponData);
            this.ComponentManager.Initialise(this.PossibleComponents);
        }

        public void AddComponent(WeaponComponentData component, int index) {
            this.ComponentManager.AddComponent(component, index);
        }
        
        public void RemoveComponent(int index) {
            this.ComponentManager.RemoveComponent(index);
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

        protected virtual void Hit(Vector3 at) {
            if (!this.WeaponData.ParticleEffectOnHit) {
                return;
            }

            ObjectSpawner.Pull(
                this.WeaponData.ParticleEffectOnHit.PoolableId, this.WeaponData.ParticleEffectOnHit, at,
                Quaternion.identity
            );
        }

        public abstract void DealDamage(Combatant combatant, ICollection<string> tags, LayerMask mask, Vector3 forward);
        
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
