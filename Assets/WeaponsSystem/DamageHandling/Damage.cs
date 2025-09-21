using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace WeaponsSystem.DamageHandling {
    public sealed class Damage {
        public GameObject Instigator { get; }
        public IReadOnlyDictionary<string, int> Data { get; }
        
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
