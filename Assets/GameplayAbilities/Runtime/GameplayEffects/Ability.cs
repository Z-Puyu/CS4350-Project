using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Targeting;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.GameplayEffects {
    [CreateAssetMenu(fileName = "New Ability", menuName = "Gameplay Abilities/Ability")]
    public class Ability : ScriptableObject {
        [field: SerializeField] internal bool IsObtainable { get; private set; } = true;
        [field: SerializeField] private string Id { get; set; }
        [field: SerializeField] private string Name { get; set; }
        [field: SerializeField] private string Description { get; set; }
        [field: SerializeField, MinValue(0)] private double Cooldown { get; set; }
        [field: SerializeReference] public TargetingStrategy TargetingStrategy { get; set; }

        [field: SerializeReference]
        private List<IEffect<AbilityEffectData, AttributeSet>> Effects { get; set; } =
            new List<IEffect<AbilityEffectData, AttributeSet>>();
        
        public double Duration => this.Effects.Count > 0 ? this.Effects.Max(effect => effect.EffectDuration) : 0;
        public double MinTimeUntilNextUse => this.Cooldown + this.Duration;
        
        public void Use(IAttributeReader instigator, AttributeSet target) {
            foreach (IEffect<AbilityEffectData, AttributeSet> effect in this.Effects) {
                effect.Apply(new AbilityEffectData(instigator), target).Start();
            }
        }

        public void Activate(AbilityCaster caster, AbilityTargeter targeter) {
            this.TargetingStrategy.Start(caster, this, targeter);
        }
    }
}
