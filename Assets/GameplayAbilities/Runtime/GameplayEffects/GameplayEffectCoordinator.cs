using System.Collections;
using System.Collections.Generic;
using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilities.Runtime.GameplayEffects {
    [DisallowMultipleComponent, RequireComponent(typeof(AttributeSet))]
    public class GameplayEffectCoordinator : MonoBehaviour {
        private AttributeSet AttributeSet { get; set; }
        
        private Dictionary<GameplayEffect, Coroutine> ActiveEffects { get; } =
            new Dictionary<GameplayEffect, Coroutine>();
        
        private Dictionary<GameplayEffect, IAbility> SourceAbilities { get; } =
            new Dictionary<GameplayEffect, IAbility>();

        internal event UnityAction<GameplayEffect, IAbility> OnEffectEnded; 

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
                    this.End(effect);
                } else {
                    yield return new WaitForSeconds(period);
                    elapsed += period;
                }
            }
        }
        
        private IEnumerator ExecuteContinuously(GameplayEffect effect, float duration) {
            yield return new WaitForSeconds(duration);
            this.End(effect);
        }
        
        private void AddPeriodicEffect(GameplayEffect effect) {
            float period = effect.Data.Period;
            if (effect.Data.Period <= 0) {
                Debug.LogWarning(
                    $"Periodic effect {effect.Data} has non-positive period, reverted to 1 second."
                );
                
                period = 1f;
            }
                    
            Coroutine periodicCoroutine = this.StartCoroutine(
                this.ExecutePeriodically(effect, period, effect.Data.ActualDuration)
            );
                    
            this.ActiveEffects.Add(effect, periodicCoroutine);
        }
        
        private void AddContinuousEffect(GameplayEffect effect) {
            effect.Apply(this.AttributeSet);
            switch (effect.Data.ActualDuration) {
                case > 0: {
                    Coroutine continuousCoroutine = this.StartCoroutine(
                        this.ExecuteContinuously(effect, effect.Data.ActualDuration)
                    );
                            
                    this.ActiveEffects.Add(effect, continuousCoroutine);
                    break;
                }
                case < 0:
                    this.ActiveEffects.Add(effect, null);
                    break;
            }
        }

        public void Add(GameplayEffect effect, int chance, IAbility ability = null) {
            if (effect.Commit(this.AttributeSet, chance) != GameplayEffect.Outcome.Success) {
                this.OnEffectEnded?.Invoke(effect, ability);
                return;
            }
            
            if (ability != null) {
                this.SourceAbilities.Add(effect, ability);
            }
            
            switch (effect.Data.ExecutionTime) {
                case GameplayEffectData.Periodicity.Periodic:
                    this.AddPeriodicEffect(effect);
                    break;
                case GameplayEffectData.Periodicity.Continuous:
                    this.AddContinuousEffect(effect);
                    break;
                default:
                    effect.Apply(this.AttributeSet);
                    this.End(effect);
                    break;
            }
        }

        internal void End(GameplayEffect effect) {
            if (this.ActiveEffects.Remove(effect)) {
                effect.Revert(this.AttributeSet);
            }
            
            if (this.SourceAbilities.Remove(effect, out IAbility ability)) {
                this.OnEffectEnded?.Invoke(effect, ability);
            } else {
                this.OnEffectEnded?.Invoke(effect, null);
            }
        }

        public GameplayEffectExecutionArgs.Builder CreateEffectExecutionArgs() {
            return GameplayEffectExecutionArgs.From(this.AttributeSet, this.transform.position);
        }
    }
}
