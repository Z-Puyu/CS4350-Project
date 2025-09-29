using System.Collections;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities {
    public class ContinuousAbilityEffect : ContinuousEffect<IDataReader<string, int>, AttributeSet> {
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
        
        public override IRunnableEffect Apply(IDataReader<string, int> source, AttributeSet target) {
            return new Instance(this.Effect.Apply(source, target), target, this.Duration);
        }
    }
}
