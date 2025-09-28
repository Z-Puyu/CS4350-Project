using System;
using System.Collections;
using System.Collections.Generic;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.GameplayEffects {
    [Serializable]
    public abstract class PeriodicEffect<S, T> : IEffect<S, T> {
        [field: SerializeField] protected double Duration { get; private set; }
        [field: SerializeField] protected float Period { get; private set; }
        [field: SerializeReference] protected IEffect<S, T> Effect { get; private set; }
        
        public double EffectDuration => this.Duration;

        protected virtual bool HasExpired(double elapsedTime) {
            return this.Duration >= 0 && elapsedTime >= this.Duration;
        }
        
        public abstract IRunnableEffect Apply(S source, T target);
    }
}
