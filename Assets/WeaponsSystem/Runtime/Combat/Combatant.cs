using System.Collections;
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
        [field: SerializeField]
        private Weapon Weapon { get; set; }
        private bool IsAttacking { get; set; }
        
        [field: SerializeField] private float swingAngle = 180f;
        [field: SerializeField] private float swingDuration = 0.25f;
        private Coroutine swingRoutine;
        private bool isSwinging = false;
        
        [field: SerializeField] private TrailRenderer SwingTrail { get; set; }

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
            
            if (SwingTrail)
            { 
                SwingTrail.Clear();
                SwingTrail.emitting = true;   // Start emitting
            }
            
            if (this.Weapon.CurrentComboIndex < 0) {
                this.IsAttacking = false;
            } else {
                this.OnAttacked.Invoke(this.Weapon.CurrentComboIndex);
                
                // Start swing
                if (swingRoutine != null) StopCoroutine(swingRoutine);
                swingRoutine = StartCoroutine(SwingWeapon());
            }
        }

        public void PerformAttack(Vector3 forward) {
            if (!this.IsAttacking) {
                return;
            }
            
            Vector3 pos = this.AttackOrigin.position;
            Vector3 direction = (pos - this.Owner.transform.position).normalized;
            this.Weapon.Attack(this.Owner, this.EnemyTags, this.EnemyLayerMask, pos, direction);
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
            
            this.Weapon.gameObject.SetActive(false);
            this.Weapon = weapon;
            Debug.Log($"Equipped {weapon.name}");
            this.IsAttacking = false;
            weapon.gameObject.SetActive(true);
            this.OnSwitchedGear.Invoke(weapon);
            return true;
        }

        public void HandleComponentSetChange(ISet<WeaponComponent> components) {
            this.OnComponentSetChanged.Invoke(components);
        }
        
        private IEnumerator SwingWeapon()
        {
            if (!AttackOrigin || !Owner.transform) yield break;

            isSwinging = true;

            // Capture starting direction (from player to AttackOrigin)
            Vector3 pivot = Owner.transform.position;
            Vector3 startDirection = (AttackOrigin.position - pivot).normalized;
            float radius = Vector3.Distance(pivot, AttackOrigin.position);

            // Capture starting rotation so we can swing relative to it
            Quaternion startRotation = AttackOrigin.rotation;
            float halfSwing = swingAngle / 2f;
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / swingDuration;
                float currentAngle = Mathf.Lerp(-halfSwing, halfSwing, t);

                // Compute new rotated direction around player
                Quaternion rot = Quaternion.AngleAxis(currentAngle, Vector3.forward);
                Vector3 newDir = rot * startDirection;

                // Update position & rotation
                AttackOrigin.position = pivot + newDir * radius;
                AttackOrigin.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(newDir.y, newDir.x) * Mathf.Rad2Deg - 90f);

                yield return null;
            }
            
            if (SwingTrail)
            { 
                SwingTrail.emitting = false;
            }
            
            isSwinging = false;
        }

    }
}