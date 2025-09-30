using System.Collections.Generic;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;
using WeaponsSystem.WeaponComponents;
using Timer = DataStructuresForUnity.Runtime.Utilities.Timer;

namespace Weapons.Runtime {
    /// <summary>
    /// This component is responsible for triggering and managing combat animations,
    /// and invoke damage logic in the weapons.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Combatant : MonoBehaviour {
        [field: SerializeField] private LayerMask EnemyLayerMask { get; set; }
        [field: SerializeField, Tag] private List<string> EnemyTags { get; set; } = new List<string>();
        [field: SerializeField] private UnityEvent<int> OnAttacked { get; set; } = new UnityEvent<int>();

        [field: SerializeField]
        private UnityEvent<Weapon> OnSwitchedGear { get; set; } = new UnityEvent<Weapon>();

        [field: SerializeField]
        private UnityEvent<ISet<AttributeBasedWeaponComponent>> OnComponentSetChanged { get; set; } =
            new UnityEvent<ISet<AttributeBasedWeaponComponent>>();
        
        private Timer AttackTimer { get; set; }
        
        private Weapon Weapon { get; set; }
        private bool IsAttacking { get; set; }

        private void Update() {
            this.AttackTimer?.Tick();
        }

        public void StartAttack() {
            if (this.IsAttacking) {
                return;
            }
            
            this.IsAttacking = true;
            this.Weapon.StartAttack();
            int combo = this.Weapon.ComboController.CurrentComboIndex;
            if (combo < 0) {
                this.IsAttacking = false;
            } else {
                this.OnAttacked.Invoke(combo);
            }
        }

        public void DealDamage(Vector3 attackPosition, Vector3 attackDirection) {
            this.Weapon.PerformAttack(this.EnemyTags, this.EnemyLayerMask, attackPosition, attackDirection);
        }

        public void QueryFinishAttack() {
            this.AttackTimer = new Timer(this.Weapon.WaitingTimeUntilNextAttack);
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
            this.OnSwitchedGear.Invoke(weapon);
            return true;
        }

        public void HandleComponentSetChange(ISet<AttributeBasedWeaponComponent> components) {
            this.OnComponentSetChanged.Invoke(components);
        }
    }
}
