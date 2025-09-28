using System.Collections.Generic;
using System.Collections.ObjectModel;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using UnityEngine;

namespace WeaponsSystem.DamageHandling {
    public sealed class Damage : IAbility {
        public (GameObject root, Combatant combatant) Instigator { get; set; }
        public IReadOnlyDictionary<string, int> Data { get; }
        
        private IEffect<IDataReader<string, int>, AttributeSet> effect;

        public Damage(GameObject instigator, Combatant combatant) {
            this.Instigator = (instigator, combatant);
        }
        
        /// <summary>
        /// Creates a new Damage object.
        /// </summary>
        /// <param name="instigator">The source of the damage. Should be the root game object.</param>
        /// <param name="combatant">The combatant component.</param>
        /// <param name="data">The damage data. The keys are IDs of damage attributes
        /// and the values are the magnitudes.</param>
        public Damage(GameObject instigator, Combatant combatant, IReadOnlyDictionary<string, int> data) {
            this.Instigator = (instigator, combatant);
            this.Data = data;
        }

        /// <summary>
        /// Creates a new Damage object.
        /// </summary>
        /// <param name="instigator">The source of the damage. Should be the root game object.</param>
        /// <param name="combatant">The combatant component.</param>
        /// <param name="data">The damage data. The keys are IDs of damage attributes
        /// and the values are the magnitudes.</param>
        public Damage(GameObject instigator, Combatant combatant, IDictionary<string, int> data) {
            this.Instigator = (instigator, combatant);
            this.Data = new ReadOnlyDictionary<string, int>(data);
        }

        public void Execute(IAttributeReader instigator, AttributeSet target) {
            this.effect.Apply(new AbilityEffectData(instigator), target);
        }
    }
}
