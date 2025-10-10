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
        
        private Weapon Weapon { get; set; }
        private bool IsAttacking { get; set; }
        
        [field: SerializeField] private float swingAngle = 180f;
        [field: SerializeField] private float swingDuration = 0.25f;
        private Coroutine swingRoutine;
        private bool isSwinging = false;

        private void Awake() {
            if (!this.Owner) {
                this.Owner = this.gameObject;
            }
        }

        private void Update() {
            this.AttackTimer?.Tick();
        }
        
        private void LateUpdate()
        {
            if (!isSwinging) // only aim at mouse when not swinging
                AimWeaponAtMouse();
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
        
        private void AimWeaponAtMouse() {
            if (!AttackOrigin) return;

            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = mouseWorld - AttackOrigin.position;
            direction.z = 0f;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // If sprite points UP and hilt is pivot, subtract 90 degrees
            float finalAngle = angle - 90f;

            // If facing left (mouse on left side), rotate an extra 180° so the tip still faces the cursor
            if (angle > 90f || angle < -90f)
            {
                finalAngle += 180f;
            }

            AttackOrigin.rotation = Quaternion.Euler(0f, 0f, finalAngle);
        }
        
        private Quaternion GetMouseRotation()
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = mouseWorld - AttackOrigin.position;
            direction.z = 0f;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float finalAngle = angle - 90f;
            if (angle > 90f || angle < -90f) finalAngle += 180f;
            return Quaternion.Euler(0f, 0f, finalAngle);
        }
        
        private IEnumerator SwingWeapon()
        {
            if (!AttackOrigin || !Owner.transform) yield break;

            isSwinging = true;

            // Freeze mouse rotation at the moment attack starts
            Quaternion mouseRotAtStart = GetMouseRotation();

            // Swing arc start and end relative to mouse direction
            float halfSwing = swingAngle / 2f;
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / swingDuration;

                // Calculate current swing angle
                float currentAngle = Mathf.Lerp(-halfSwing, halfSwing, t);

                // Set AttackOrigin position at character center
                AttackOrigin.position = Owner.transform.position;

                // Compute rotation for this frame
                Quaternion swingRot = mouseRotAtStart * Quaternion.Euler(0f, 0f, currentAngle);

                // Rotate AttackOrigin around character center
                AttackOrigin.RotateAround(Owner.transform.position, Vector3.forward, currentAngle);

                yield return null;
            }

            // Swing finished — return control to mouse
            isSwinging = false;
        }
    }
}