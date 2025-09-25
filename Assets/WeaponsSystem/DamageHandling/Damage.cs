using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GameplayAbilities.Runtime.Abilities;
using UnityEngine;

namespace WeaponsSystem.DamageHandling {
    public sealed class Damage {
        public GameObject Instigator { get; }
        public IReadOnlyDictionary<string, int> Data { get; }
        public HashSet<IAbility> EffectsOnTarget { get; } = new HashSet<IAbility>();
        public HashSet<IAbility> EffectsOnSelf { get; } = new HashSet<IAbility>();

        public Damage(GameObject instigator) {
            this.Instigator = instigator;
        }

        public Damage WithEffectOnTarget(IAbility effect) {
            if (!this.EffectsOnTarget.Add(effect)) {
                Debug.LogError("Cannot have duplicate effects in one damage!");
            }
            
            return this;
        }
        
        public Damage WithEffectsOnTarget(IEnumerable<IAbility> effects) {
            foreach (IAbility effect in effects) {
                if (!this.EffectsOnTarget.Add(effect)) {
                    Debug.LogError("Cannot have duplicate effects in one damage!");
                }
            }
            
            return this;
        }

        public Damage WithEffectOnSelf(IAbility effect) {
            if (!this.EffectsOnSelf.Add(effect)) {
                Debug.LogError("Cannot have duplicate effects in one damage!");
            }
            
            return this;
        }
        
        public Damage WithEffectsOnSelf(IEnumerable<IAbility> effects) {
            foreach (IAbility effect in effects) {
                if (!this.EffectsOnSelf.Add(effect)) {
                    Debug.LogError("Cannot have duplicate effects in one damage!");
                }
            }
            
            return this;
        }
        
        /// <summary>
        /// Creates a new Damage object.
        /// </summary>
        /// <param name="instigator">The source of the damage. Should be the root game object.</param>
        /// <param name="data">The damage data. The keys are IDs of damage attributes
        /// and the values are the magnitudes.</param>
        public Damage(GameObject instigator, IReadOnlyDictionary<string, int> data) {
            this.Instigator = instigator;
            this.Data = data;
        }

        /// <summary>
        /// Creates a new Damage object.
        /// </summary>
        /// <param name="instigator">The source of the damage. Should be the root game object.</param>
        /// <param name="data">The damage data. The keys are IDs of damage attributes
        /// and the values are the magnitudes.</param>
        public Damage(GameObject instigator, IDictionary<string, int> data) {
            this.Instigator = instigator;
            this.Data = new ReadOnlyDictionary<string, int>(data);
        }
    }
}
