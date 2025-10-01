using System;
using System.Collections.Generic;
using System.Linq;
using DataStructuresForUnity.Runtime.ObjectPooling;
using UnityEngine;

namespace Projectiles.Runtime {
    [DisallowMultipleComponent]
    public abstract class Projectile : PoolableObject<Projectile> {
        private Transform Transform { get; set; }
        protected Vector3 LaunchPoint { get; private set; }
        protected Vector3 Direction { get; set; }
        protected float Speed { get; set; }
        protected double Range { get; set; }
        private List<string> TargetTags { get; } = new List<string>();
        private bool IsAlive { get; set; }

        [field: SerializeReference]
        private List<IProjectileController> Controllers { get; set; } = new List<IProjectileController>();

        private Dictionary<Type, IProjectileController> ControllersByType { get; } =
            new Dictionary<Type, IProjectileController>();
        
        public event Action<Vector3, GameObject> OnHit;
        
        protected void Awake() {
            this.Transform = this.transform;
            this.Controllers.ForEach(controller => this.ControllersByType.Add(controller.GetType(), controller));
        }

        private void OnEnable() {
            this.Controllers.ForEach(controller => controller.Possess(this));
        }

        private void OnDisable() {
            this.Controllers.ForEach(controller => controller.ReleaseControl());
        }

        #region Builder Methods

        public Projectile Targeting(IEnumerable<string> tags) {
            foreach (string t in tags) {
                if (this.TargetTags.Contains(t)) {
                    continue;
                }
                
                this.TargetTags.Add(t);
            }

            return this;
        }

        public Projectile Targeting(params string[] tags) {
            foreach (string t in tags) {
                if (this.TargetTags.Contains(t)) {
                    continue;
                }
                
                this.TargetTags.Add(t);
            }

            return this;
        }

        public Projectile WhenHit(Action<Vector3, GameObject> action) {
            this.OnHit += action;
            return this;
        }

        public void Launch(Vector3 direction, float speed, double range) {
            this.Launch(this.transform.position, direction, speed, range);       
        }
        
        public void Launch(Vector3 from, Vector3 direction, float speed, double range) {
            this.Direction = direction;
            this.Speed = speed;
            this.Range = range;
            this.transform.position = from;
            this.LaunchPoint = from;
            this.IsAlive = true;       
        }

        public Projectile Relaunch() {
            this.IsAlive = true;
            return this;
        }

        public Projectile Relaunch(Vector3 direction, Vector3 relaunchPoint, float speed, double range) {
            this.Direction = direction;
            this.Speed = speed;
            this.Range = range;
            this.IsAlive = true;
            this.LaunchPoint = relaunchPoint;
            return this;
        }

        #endregion

        public override void Return() {
            this.StopAllCoroutines();
            this.TargetTags.Clear();
            this.Speed = 0;
            this.Range = 0;
            this.Direction = Vector3.zero;
            this.LaunchPoint = Vector3.zero;
            this.OnHit = null;
            base.Return();
        }
        
        public C GetController<C>() where C : class, IProjectileController, new() {
            Type type = typeof(C);
            if (this.ControllersByType.TryGetValue(type, out IProjectileController controller)) {
                return controller as C;
            }

            controller = new C();
            this.ControllersByType.Add(type, controller);
            this.Controllers.Add(controller);
            return controller as C;
        }

        public virtual bool IsValidTarget(GameObject candidate) {
            return this.TargetTags.Count == 0 || this.TargetTags.Any(candidate.CompareTag);
        }

        protected void Hit(GameObject target) {
            this.IsAlive = false;
            this.Impact(target);
            this.OnHit?.Invoke(target.transform.position, target);
            if (!this.IsAlive) {
                this.Return();
            }
        }

        protected void Impact(GameObject target) {
            this.IsAlive = false;
            Vector3 position = target.transform.position;
            this.Controllers.ForEach(controller => controller.ProcessHit(position, target));
            this.OnHit?.Invoke(position, target);
            if (!this.IsAlive) {
                this.Return();
            }       
        }

        private void Travel(float deltaTime) {
            this.Transform.position += this.Direction * (deltaTime * this.Speed);
            this.Transform.right = this.Direction;
        }
        

        protected void Update() {
            if (!this.IsAlive) {
                return;
            }
            
            float distance = Vector3.Distance(this.Transform.position, this.LaunchPoint);
            if (distance >= this.Range) {
                this.Return();
                return;
            }

            foreach (IProjectileController controller in this.Controllers) {
                controller.Update();
            }
            
            this.Travel(Time.deltaTime);
        }
    }
}
