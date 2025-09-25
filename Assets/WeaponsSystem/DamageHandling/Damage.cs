using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace WeaponsSystem.DamageHandling {
    public sealed class Damage {
        public GameObject Instigator { get; }
        public IReadOnlyDictionary<string, int> Data { get; }
        public Dictionary<Type, HashSet<object>> SpecialData { get; } = new Dictionary<Type, HashSet<object>>();
        public int TotalDamage { get; }

        public Damage(GameObject instigator) {
            this.Instigator = instigator;
            this.TotalDamage = 0;       
        }

        public Damage WithSpecialData<T>(T data) {
            Type type = typeof(T);
            if (this.SpecialData.TryGetValue(type, out HashSet<object> set)) {
                set.Add(data);
            } else {
                this.SpecialData.Add(type, new HashSet<object> { data });
            }

            return this;
        }

        public bool HasSpecialData<T>(out IEnumerable<T> data) {
            if (this.SpecialData.TryGetValue(typeof(T), out HashSet<object> set)) {
                data = set.Cast<T>();
                return data.Any();
            }
            
            data = null;       
            return false;       
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
            this.TotalDamage = data.Values.Sum();
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
            this.TotalDamage = data.Values.Sum();       
        }
    }
}
