using System;
using DataStructuresForUnity.Runtime.ObjectPooling;
using GameplayAbilities.Runtime.Attributes;
using GameplayEffects.Runtime;
using Projectiles.Runtime;
using UnityEngine;

namespace GameplayAbilities.Runtime.Projectiles {
    [Serializable]
    public abstract class ProjectileEffect : IEffect<IDataReader<string, int>, Projectile> {
        [field: SerializeField] private string Name { get; set; }
        [field: SerializeField] protected IEffect<IDataReader<string, int>, AttributeSet> Effect { get; private set; }
        [field: SerializeField] private PoolableObject SpawnOnHit { get; set; }
        public double EffectDuration => this.Effect.EffectDuration;

        public abstract IRunnableEffect Apply(IDataReader<string, int> source, Projectile projectile);

        private void Trigger(Vector3 position, GameObject obj, Projectile projectile, IDataReader<string, int> sender) {
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
