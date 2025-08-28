using System.Collections;
using System.Collections.Generic;
using GameplayAbilities.Runtime.Attributes;
using UnityEngine;

namespace GameplayAbilities.Runtime.GameplayEffects {
    [DisallowMultipleComponent, RequireComponent(typeof(AttributeSet))]
    internal class GameplayEffectCoordinator : MonoBehaviour {
        private AttributeSet AttributeSet { get; set; }
        
        private Dictionary<GameplayEffect, Coroutine> ActiveEffects { get; } =
            new Dictionary<GameplayEffect, Coroutine>();

        private void Awake() {
            this.AttributeSet = this.GetComponent<AttributeSet>();
        }

        private IEnumerator ExecutePeriodically(GameplayEffect effect, float period, float duration) {
            float elapsed = 0f;
            yield return new WaitForSeconds(period);
            elapsed += period;
            
            while (this.ActiveEffects.ContainsKey(effect) && (duration < 0 || elapsed < duration)) {
                effect.Apply(this.AttributeSet);
                if (duration < 0) {
                    yield return new WaitForSeconds(period);
                    continue;
                }

                if (elapsed >= duration) {
                    this.Remove(effect);
                } else {
                    yield return new WaitForSeconds(period);
                    elapsed += period;
                }
            }
        }
        
        private IEnumerator ExecuteContinuously(GameplayEffect effect, float duration) {
            yield return new WaitForSeconds(duration);
            this.Remove(effect);
        }

        internal void Add(GameplayEffect effect) {
            switch (effect.Data.ExecutionTime) {
                case GameplayEffectData.Periodicity.Periodic:
                    if (effect.Data.Period <= 0) {
                        Debug.LogWarning($"Periodic effect {effect.Data} has non-positive period, reverted to 1 second.");
                        return;
                    }

                    Coroutine periodicCoroutine = this.StartCoroutine(this.ExecutePeriodically(
                        effect, effect.Data.Period, effect.Data.Duration));
                    this.ActiveEffects.Add(effect, periodicCoroutine);
                    break;
                case GameplayEffectData.Periodicity.Continuous:
                    effect.Apply(this.AttributeSet);
                    Coroutine continuousCoroutine = this.StartCoroutine(this.ExecuteContinuously(
                        effect, effect.Data.Duration));
                    this.ActiveEffects.Add(effect, continuousCoroutine);
                    break;
            }
        }

        internal void Remove(GameplayEffect effect) {
            if (this.ActiveEffects.Remove(effect)) {
                effect.EndOn(this.AttributeSet);
            }
        }
    }
}
