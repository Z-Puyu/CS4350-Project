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

        private void Awake() {
            this.AttributeSet = this.GetComponent<AttributeSet>();
        }

        private IEnumerator ExecutePeriodically(GameplayEffect effect, float period, double duration) {
            double elapsed = 0;
            yield return new WaitForEndOfFrame();
            while (this.ActiveEffects.ContainsKey(effect)) {
                effect.Apply(this.AttributeSet);
#if DEBUG
                Debug.Log(
                    $"Applying effect from {this.SourceAbilities[effect]}, elapsed: {elapsed}, duration: {duration}"
                );
#endif
                if (duration < 0) {
                    yield return new WaitForSeconds(period);
                    continue;
                }

                if (elapsed >= duration) {
#if DEBUG
                    Debug.Log(
                        $"Effect ended, from {this.SourceAbilities[effect]}, elapsed: {elapsed}, duration: {duration}"
                    );
#endif
                    this.End(effect);
                    break;
                }
                
                yield return new WaitForSeconds(period);
                elapsed += period;
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

            this.SourceAbilities.Remove(effect);
        }
        
        internal void End(IAbility ability) {
            List<GameplayEffect> toEnd = new List<GameplayEffect>();
            foreach (KeyValuePair<GameplayEffect, IAbility> pair in this.SourceAbilities) {
                if (pair.Value == ability) {
                    toEnd.Add(pair.Key);
                }
            }
            
            toEnd.ForEach(this.End);
        }

        public GameplayEffectExecutionArgs.Builder CreateEffectExecutionArgs() {
            return GameplayEffectExecutionArgs.From(this.AttributeSet);
        }
    }
}
