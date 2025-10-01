using System.Collections.Generic;
using DataStructuresForUnity.Runtime.Utilities;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;
using WeaponsSystem.Runtime.DamageHandling;
using WeaponsSystem.Runtime.WeaponComponents;
using WeaponsSystem.Runtime.Weapons;

namespace WeaponsSystem.Runtime.Combat {
    /// <summary>
    /// This component is responsible for triggering and managing combat animations,
    /// and invoke damage logic in the weapons.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Combatant : MonoBehaviour {
        [field: SerializeField] private GameObject Owner { get; set; }
        [field: SerializeField, Required] private Transform AttackOrigin { get; set; }
        [field: SerializeField] private LayerMask EnemyLayerMask { get; set; }
        [field: SerializeField, Tag] private List<string> EnemyTags { get; set; } = new List<string>();
        [field: SerializeField] private UnityEvent<int> OnAttacked { get; set; } = new UnityEvent<int>();

        [field: SerializeField]
        private UnityEvent<Weapon> OnSwitchedGear { get; set; } = new UnityEvent<Weapon>();

        [field: SerializeField]
        private UnityEvent<Damage, int> OnHitTarget { get; set; } = new UnityEvent<Damage, int>();

        [field: SerializeField]
        private UnityEvent<ISet<WeaponComponent>> OnComponentSetChanged { get; set; } =
            new UnityEvent<ISet<WeaponComponent>>();
        
        private Timer AttackTimer { get; set; }
        
        private Weapon Weapon { get; set; }
        private bool IsAttacking { get; set; }

        private void Awake() {
            if (!this.Owner) {
                this.Owner = this.gameObject;
            }
        }

        private void Update() {
            this.AttackTimer?.Tick();
        }

        public void StartAttack() {
            if (!this.Weapon) {
                this.Weapon = this.GetComponentInChildren<Weapon>();
            }
            
            if (this.IsAttacking) {
                return;
            }
            
            this.IsAttacking = true;
            if (this.Weapon.CurrentComboIndex < 0) {
                this.IsAttacking = false;
            } else {
                this.OnAttacked.Invoke(this.Weapon.CurrentComboIndex);
            }
        }

        public void PerformAttack(Vector3 forward) {
            Vector3 direction = (this.AttackOrigin.position - this.Owner.transform.position).normalized;
            this.Weapon.Attack(this.Owner, this.EnemyTags, this.EnemyLayerMask, this.AttackOrigin.position, direction);
        }

        public void QueryFinishAttack() {
            this.AttackTimer = new Timer(this.Weapon.AttackDuration);
            this.AttackTimer.OnTimerFinished += this.FinishAttack;
            this.AttackTimer.Start();
        }

        private void FinishAttack() {
            this.IsAttacking = false;
            this.Weapon.EndAttack();
        }

        public void Interrupt() {
            this.IsAttacking = false;       
        }
        
        public bool Equip(Weapon weapon) {
            if (weapon == this.Weapon) {
                return false;           
            }
            
            this.Weapon = weapon;
            this.IsAttacking = false;
            weapon.gameObject.SetActive(true);
            this.OnSwitchedGear.Invoke(weapon);
            return true;
        }

        public void HandleComponentSetChange(ISet<WeaponComponent> components) {
            this.OnComponentSetChanged.Invoke(components);
        }
    }
}