using System;
using System.Collections.Generic;
using System.Linq;
using DataStructuresForUnity.Runtime.GeneralUtils;
using GameplayAbilities.Runtime.GameplayEffects;
using UnityEngine;

namespace GameplayAbilities.Runtime.Projectiles {
    [DisallowMultipleComponent, RequireComponent(typeof(CapsuleCollider2D))]
    public class Projectile2D : PoolableObject {
        [field: SerializeField] private string Id { get; set; }
        private CapsuleCollider2D Collider { get; set; }
        private Ability Ability { get; set; }
        private Transform Transform { get; set; }
        private Vector3 LaunchPoint { get; set; }
        private Vector3 Direction { get; set; }
        private float Speed { get; set; }
        private double Range { get; set; }
        private List<string> TargetTags { get; } = new List<string>();
        private bool IsAlive { get; set; }
        private Action<Vector3> OnHitAction { get; set; } = delegate { };
        
        public override string PoolableId => this.Id;

        private void Awake() {
            this.Transform = this.transform;
        }

        public void Activate() {
            this.IsAlive = true;
            this.LaunchPoint = this.Transform.position;
        }
        
        public void Deactivate() {
            this.Return();
        }

        protected virtual void Hit(Collider2D target) {
            this.OnHitAction(target.transform.position);
            if (target.TryGetComponent(out IProjectileHandler handler)) {
                handler.Handle(this);
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (this.TargetTags.Count > 0 && !this.TargetTags.Any(other.gameObject.CompareTag)) {
                return;
            }

            this.Hit(other);
        }

        private void Update() {
            if (!this.IsAlive) {
                return;
            }

            float distance = Vector3.Distance(this.transform.position, this.LaunchPoint);
            if (distance >= this.Range) {
                this.Deactivate();
                return;
            }
            
            this.Transform.position += this.Direction * (Time.deltaTime * this.Speed);
            this.Transform.right = this.Direction;
        }
    }
}
