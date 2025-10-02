using System.Collections.Generic;
using System.Linq;
using DataStructuresForUnity.Runtime.Utilities;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Targeting;
using GameplayEffects.Runtime;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities {
    [CreateAssetMenu(fileName = "New Ability", menuName = "Gameplay Abilities/Ability")]
    public class Ability : ScriptableObject, IAbility {
        [field: SerializeField] internal bool IsObtainable { get; private set; } = true;
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField, MinValue(0)] private double Cooldown { get; set; }
        [field: SerializeField] private List<AbilityCost> Costs { get; set; } = new List<AbilityCost>();
        [field: SerializeReference] public TargetingStrategy TargetingStrategy { get; set; }

        [field: SerializeReference]
        private List<IEffect<IDataReader<string, int>, AttributeSet>> Effects { get; set; } =
            new List<IEffect<IDataReader<string, int>, AttributeSet>>();
        
        public double Duration => this.Effects.Count > 0 ? this.Effects.Max(effect => effect.EffectDuration) : 0;
        public double MinTimeUntilNextUse => this.Cooldown + this.Duration;

        public bool IsFeasible(IAttributeReader instigator, AttributeSet target) {
            return this.Costs.TrueForAll(cost => cost.IsAffordable(instigator));
        }

        public void Execute(IAttributeReader instigator, AttributeSet target) {
            foreach (IEffect<AttributeDataReader, AttributeSet> effect in this.Effects) {
                effect.Apply(new AttributeDataReader(instigator), target).Start();
            }
        }

        /// <summary>
        /// Starts targeting for the ability.
        /// </summary>
        /// <param name="caster">The caster.</param>
        /// <param name="targeter">The targeter component.</param>
        public void Activate(AbilityCaster caster, AbilityTargeter targeter) {
            this.TargetingStrategy.Start(caster, this, targeter);
        }
    }
}
