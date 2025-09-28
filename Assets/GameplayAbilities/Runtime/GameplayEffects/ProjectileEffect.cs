using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Projectiles;
using UnityEngine;

namespace GameplayAbilities.Runtime.GameplayEffects {
    public class ProjectileEffect : IEffect<IAttributeReader, IProjectileHandler> {
        [field: SerializeField] private IEffect<AbilityEffectData, AttributeSet> Effect { get; set; }
        public double EffectDuration => this.Effect.EffectDuration;
        
        public IRunnableEffect Apply(IAttributeReader source, IProjectileHandler target) {
            return this.Effect.Apply(new AbilityEffectData(source), target.AttributeSet);
        }
    }
}
