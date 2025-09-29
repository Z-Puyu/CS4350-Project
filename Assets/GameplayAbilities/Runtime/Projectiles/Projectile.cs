using System;
using System.Collections.Generic;
using System.Linq;
using DataStructuresForUnity.Runtime.GeneralUtils;
using DataStructuresForUnity.Runtime.ObjectPooling;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using UnityEngine;

namespace GameplayAbilities.Runtime.Projectiles {
    [DisallowMultipleComponent]
    public abstract class Projectile : PoolableObject {
        [field: SerializeField] private string Id { get; set; }
        private IAbility Ability { get; set; }
        private IAttributeReader Sender { get; set; }
        protected Transform Transform { get; private set; }
        protected Vector3 LaunchPoint { get; private set; }
        protected Vector3 Direction { get; set; }
        protected float Speed { get; set; }
        protected double Range { get; set; }
        private List<string> TargetTags { get; } = new List<string>();
        private bool IsAlive { get; set; }
        
        public event Action<Vector3, GameObject> OnHit;
        
        protected void Awake() {
            this.Transform = this.transform;
        }

        public override void Initialise(Action<PoolableObject> onReturn) {
            base.Initialise(onReturn);
            this.IsAlive = true;
            this.LaunchPoint = this.Transform.position;
        }
        
        public override void Return() {
            this.OnHit = null;
            this.StopAllCoroutines();
            base.Return();
        }
        
        public int GetAttribute(string key) {
            return this.Sender.GetCurrent(key);
        }

        public bool IsValidTarget(Component component) {
            return this.TargetTags.Count == 0 || this.TargetTags.Any(component.CompareTag);
        }

        protected void Hit(GameObject target) {
            if (target.TryGetComponent(out IProjectileHandler handler)) {
                handler.Handle(this);
            }
            
            this.OnHit?.Invoke(target.transform.position, target);
        }

        public void Impact(AttributeSet target) {
            this.Ability.Execute(this.Sender, target);
        }

        protected abstract void OnUpdate();

        protected void Update() {
            if (!this.IsAlive) {
                return;
            }
            
            this.OnUpdate();
        }
    }
}
