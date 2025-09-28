using System;
using System.Collections;
using System.Collections.Generic;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.GameplayEffects {
    [Serializable]
    public abstract class ContinuousEffect<S, T> : IEffect<S, T> {
        [field: SerializeField, MinValue(0)] protected float Duration { get; private set; }
        [field: SerializeReference] protected IEffect<S, T> Effect { get; private set; }

        public double EffectDuration => this.Duration;
        
        public abstract IRunnableEffect Apply(S source, T target);
    }
}
