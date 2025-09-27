using System.Collections.Generic;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;
using WeaponsSystem.WeaponComponent;
using Timer = Utilities.Timer;

namespace WeaponsSystem.DamageHandling {
    /// <summary>
    /// This component is responsible for triggering and managing combat animations,
    /// and invoke damage logic in the weapons.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Combatant : MonoBehaviour {
        [field: SerializeField] private SaintsInterface<Component, IDamageDealer> DefaultDamageDealer { get; set; }
        [field: SerializeField] private LayerMask EnemyLayerMask { get; set; }
        [field: SerializeField, Tag] private List<string> EnemyTags { get; set; } = new List<string>();
        [field: SerializeField] private UnityEvent<int> OnAttacked { get; set; } = new UnityEvent<int>();

        [field: SerializeField]
        private UnityEvent<IDamageDealer> OnSwitchedGear { get; set; } = new UnityEvent<IDamageDealer>();

        [field: SerializeField]
        private UnityEvent<Damage, int> OnHitTarget { get; set; } = new UnityEvent<Damage, int>();

        [field: SerializeField]
        private UnityEvent<ISet<WeaponComponentData>> OnComponentSetChanged { get; set; } =
            new UnityEvent<ISet<WeaponComponentData>>();
        
        private Timer AttackTimer { get; set; }
        
        private IDamageDealer DamageDealer { get; set; }
        private bool IsAttacking { get; set; }

        private void Start() {
            if (this.DefaultDamageDealer.I != null) {
                this.Equip(this.DefaultDamageDealer.I);
            }
        }

        private void Update() {
            this.AttackTimer?.Tick();
        }

        public void StartAttack() {
            if (this.IsAttacking) {
                return;
            }
            
            this.IsAttacking = true;
            int combo = this.DamageDealer.StartAttack();
            if (combo < 0) {
                this.IsAttacking = false;
            } else {
                this.OnAttacked.Invoke(combo);
            }
        }

        public void DealDamage(Vector3 forward) {
            this.DamageDealer.DealDamage(this, this.EnemyTags, this.EnemyLayerMask, forward);
        }

        public void QueryFinishAttack() {
            this.AttackTimer = new Timer(this.DamageDealer.AttackDuration);
            this.AttackTimer.OnTimerFinished += this.FinishAttack;
            this.AttackTimer.Start();
        }

        private void FinishAttack() {
            this.IsAttacking = false;
            this.DamageDealer.EndAttack();
        }

        public void Interrupt() {
            this.IsAttacking = false;       
        }
        
        public bool Equip(IDamageDealer damageDealer) {
            if (damageDealer == this.DamageDealer) {
                return false;           
            }
            
            this.DamageDealer = damageDealer;
            this.IsAttacking = false;
            this.OnSwitchedGear.Invoke(damageDealer);
            return true;
        }

        public void HandleComponentSetChange(ISet<WeaponComponentData> components) {
            this.OnComponentSetChanged.Invoke(components);
        }
    }
}
