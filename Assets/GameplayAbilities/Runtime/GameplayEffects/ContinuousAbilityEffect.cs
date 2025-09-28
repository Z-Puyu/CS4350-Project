using System;
using System.Collections;
using System.Collections.Generic;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.GameplayEffects {
    public class ContinuousAbilityEffect : ContinuousEffect<AbilityEffectData, AttributeSet> {
        private class Instance : IRunnableEffect {
            private AttributeSet Target { get; }
            private IRunnableEffect InnerEffect { get; }
            private Coroutine Coroutine { get; set; }
            private float Duration { get; }

            public Instance(IRunnableEffect innerEffect, AttributeSet target, float duration) {
                this.InnerEffect = innerEffect;
                this.Target = target;
                this.Coroutine = null;
                this.Duration = duration;
            }

            public void Start() {
                this.Coroutine = this.Target.StartCoroutine(this.ApplyContinuously());
            }

            public void Stop() {
                this.Target.StopCoroutine(this.Coroutine);
                this.InnerEffect.Cancel();
                this.InnerEffect.Stop();
                this.Coroutine = null;
            }
            
            public void Cancel() {
                this.Target.StopCoroutine(this.Coroutine);
                this.InnerEffect.Cancel();
                this.Coroutine = null;
            }
            
            private IEnumerator ApplyContinuously() {
                this.InnerEffect.Start();
                yield return new WaitForSeconds(this.Duration);
                this.InnerEffect.Stop();
            }
        }
        
        public override IRunnableEffect Apply(AbilityEffectData source, AttributeSet target) {
            return new Instance(this.Effect.Apply(source, target), target, this.Duration);
        }
    }
}
