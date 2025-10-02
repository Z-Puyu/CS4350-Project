using System;
using SaintsField;
using UnityEngine;

namespace GameplayEffects.Runtime {
    [Serializable]
    public abstract class ContinuousEffect<S, T> : IEffect<S, T> {
        [field: SerializeField, MinValue(0)] protected float Duration { get; private set; }
        [field: SerializeReference] protected IEffect<S, T> Effect { get; private set; }

        public double EffectDuration => this.Duration;
        
        public abstract IRunnableEffect Apply(S source, T projectile);
    }
}
