using System.Collections;
using System.Collections.Generic;
using GameplayAbilities.Runtime.Attributes;
using UnityEngine;

namespace GameplayAbilities.Runtime.GameplayEffects {
    public sealed class PeriodicAbilityEffect : PeriodicEffect<AbilityEffectData, AttributeSet> {
        private class Instance : IRunnableEffect {
            private AttributeSet Target { get; }
            private IRunnableEffect InnerEffect { get; }
            private Coroutine Coroutine { get; set; }
            private double Duration { get; }
            private float Period { get; }
            private int ApplicationCount { get; set; }

            public Instance(IRunnableEffect innerEffect, AttributeSet target, double duration, float period) {
                this.InnerEffect = innerEffect;
                this.Target = target;
                this.Coroutine = null;
                this.Duration = duration;
                this.Period = period;
            }

            public void Start() {
                this.Coroutine = this.Target.StartCoroutine(this.ApplyPeriodically());
            }

            public void Stop() {
                this.Target.StopCoroutine(this.Coroutine);
                this.InnerEffect.Stop();
                this.Coroutine = null;
            }
            
            public void Cancel() {
                this.Target.StopCoroutine(this.Coroutine);
                for (int i = 0; i < this.ApplicationCount; i += 1) {
                    this.InnerEffect.Cancel();
                }

                this.Coroutine = null;
            }
            
            private IEnumerator ApplyPeriodically() {
                double elapsed = 0;
                this.InnerEffect.Start();
                this.ApplicationCount += 1;
#if DEBUG
                Debug.Log($"Applying effect, elapsed: {elapsed} / {this.Duration}");
#endif
                while (this.Duration < 0 || elapsed < this.Duration) {
                    if (this.Duration >= 0) {
                        elapsed += this.Period;
                    }

                    yield return new WaitForSeconds(this.Period);
#if DEBUG
                    Debug.Log($"Applying effect, elapsed: {elapsed} / {this.Duration}");
#endif
                    this.InnerEffect.Start();
                    this.ApplicationCount += 1;
                }
#if DEBUG
                Debug.Log($"Effect ended, elapsed: {elapsed} / {this.Duration}");
#endif
            }
        }
        
        public override IRunnableEffect Apply(AbilityEffectData source, AttributeSet target) {
            return new Instance(this.Effect.Apply(source, target), target, this.Duration, this.Period);
        }
    }
}
