using System;
using DataStructuresForUnity.Runtime.GeneralUtils;
using DataStructuresForUnity.Runtime.ObjectPooling;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using UnityEngine;

namespace GameplayAbilities.Runtime.Projectiles {
    [Serializable]
    public abstract class ProjectileEffect : IEffect<IDataReader<string, int>, Projectile> {
        private readonly struct Instance : IRunnableEffect {
            private Projectile Projectile { get; }
            private Action<Vector3, GameObject, Projectile, IDataReader<string, int>> OnHit { get; }
            private IDataReader<string, int> Sender { get; }

            public Instance(
                Projectile projectile, Action<Vector3, GameObject, Projectile, IDataReader<string, int>> onHit,
                IDataReader<string, int> sender
            ) {
                this.Projectile = projectile;
                this.OnHit = onHit;
                this.Sender = sender;
            }

            public void Start() {
                this.Projectile.OnHit += this.HandleHit;
            }

            public void Stop() {
                this.Projectile.OnHit -= this.HandleHit;
            }

            public void Cancel() {
                this.Stop();
            }

            private void HandleHit(Vector3 position, GameObject target) {
                this.OnHit(position, target, this.Projectile, this.Sender);
            }
        }

        [field: SerializeField] private string Name { get; set; }
        [field: SerializeField] protected IEffect<IDataReader<string, int>, AttributeSet> Effect { get; private set; }
        [field: SerializeField] private PoolableObject SpawnOnHit { get; set; }
        public double EffectDuration => this.Effect.EffectDuration;

        public IRunnableEffect Apply(IDataReader<string, int> source, Projectile target) {
            return new Instance(target, this.Trigger, source);
        }

        public void Trigger(Vector3 position, GameObject obj, Projectile projectile, IDataReader<string, int> sender) {
            if (!obj) {
                ObjectPools<PoolableObject>.Get(this.SpawnOnHit, position, projectile.transform);
            } else {
                ObjectPools<PoolableObject>.Get(this.SpawnOnHit, position, obj.transform);
            }

            this.HandleHit(position, obj, projectile, sender);
        }

        protected abstract void HandleHit(
            Vector3 position, GameObject obj, Projectile projectile, IDataReader<string, int> sender
        );
    }
}
