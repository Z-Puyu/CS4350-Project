using System;
using UnityEngine;

namespace GameplayAbilities.Runtime.Projectiles {
    [Serializable]
    public class ProjectileEffectHandler : IProjectileEffectHandler {
        [field: SerializeField, Required] private AttributeSet AttributeSet { get; set; }
        
        public void Handle(IEffect<IDataReader<string, int>, AttributeSet> effect, IDataReader<string, int> sender) {
            effect.Apply(sender, this.AttributeSet).Start();
        }
    }
}
