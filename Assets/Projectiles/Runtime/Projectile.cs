using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataStructuresForUnity.Runtime.ObjectPooling;
using UnityEngine;
using UnityEngine.Serialization;

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
        [field: SerializeField] private GameObject Visual { get;set; }
        [field: SerializeField] private ParticleSystem FlyParticles { get; set; }
        [field: SerializeField] private ParticleSystem DisintegrationParticles { get; set; }
        
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
            this.Visual.SetActive(true);
            this.Controllers.ForEach(controller => controller.Possess(this));
        }

        private void OnDisable() {
            this.Visual.SetActive(false);
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

        public Projectile WhenHit<C>(Action<Vector3, GameObject> action) where C : class, IProjectileController, new() {
            this.GetController<C>().OnHit += action;
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
            this.Direction = direction.normalized;
            this.Speed = speed;
            this.Range = range;
            this.transform.position = from;
            this.LaunchPoint = from;
            this.IsAlive = true;
            if (this.FlyParticles) {
                this.FlyParticles.Play();
            }
        }

        public Projectile Relaunch() {
            this.IsAlive = true;
            return this;
        }

        public Projectile Relaunch(Vector3 direction, Vector3 relaunchPoint, float speed, double range) {
            this.Direction = direction.normalized;
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
            this.gameObject.SetActive(false);
            base.Return();
        }

        public IProjectileController GetController<C>() where C : class, IProjectileController, new() {
            Type type = typeof(C);
            if (this.ControllersByType.TryGetValue(type, out IProjectileController controller)) {
                return controller as C;
            }

            controller = new C();
            this.ControllersByType.Add(type, controller);
            this.Controllers.Add(controller);
            return controller;
        }

        public virtual bool IsValidTarget(GameObject candidate) {
            return this.TargetTags.Count == 0 || this.TargetTags.Any(candidate.CompareTag);
        }

        protected void Impact(GameObject target) {
            this.IsAlive = false;
            Vector3 position = target ? target.transform.position : this.Transform.position;
            if (target) {
                this.OnHit?.Invoke(position, target);
            }
            
            this.Controllers.ForEach(controller => controller.ProcessHit(position, target));
            if (!this.IsAlive) {
                this.StartCoroutine(this.Destroy());
            }       
        }

        private IEnumerator Destroy() {
            this.Visual.SetActive(false);
            if (this.FlyParticles) {
                this.FlyParticles.Stop();
            }

            if (this.DisintegrationParticles) {
                this.DisintegrationParticles.Play();
            }

            yield return new WaitUntil(() => (!this.FlyParticles || !this.FlyParticles.IsAlive()) &&
                                             (!this.DisintegrationParticles ||
                                              !this.DisintegrationParticles.IsAlive()) &&
                                             this.Controllers.TrueForAll(controller => controller.IsIdle));
            this.Return();
        }

        private void Travel(float deltaTime) {
            this.Transform.position += this.Direction * (deltaTime * this.Speed);
            this.Transform.right = this.Direction;
        }
        

        protected void FixedUpdate() {
            if (!this.IsAlive) {
                return;
            }
            
            float distance = Vector3.Distance(this.Transform.position, this.LaunchPoint);
            if (distance >= this.Range) {
                this.Impact(null);
                return;
            }

            foreach (IProjectileController controller in this.Controllers) {
                controller.Update();
            }

            this.Travel(Time.fixedDeltaTime);
        }
    }
}
