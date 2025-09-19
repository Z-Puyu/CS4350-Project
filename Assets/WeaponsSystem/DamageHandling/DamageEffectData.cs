using System.Collections.Generic;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.DamageHandling {
    public sealed class DamageExecution : GameplayEffectData {
        [field: SerializeField, Table]
        private List<DamageType> DamageTypes { get; set; } = new List<DamageType>();

        [field: SerializeField] private AttributeTypeDefinition TargetAttribute { get; set; }

        public override IEnumerable<Modifier> Run(AttributeSet target, GameplayEffectExecutionArgs args) {
            List<Modifier> modifiers = new List<Modifier>();
            foreach (DamageType damage in this.DamageTypes) {
                int magnitude = args.CallerSuppliedDataValues.GetValueOrDefault(damage.DamageAttribute, 0);
                if (magnitude == 0) {
                    continue;
                }
                
                int defence = target.GetCurrent(damage.DefenceAttribute);
                magnitude = damage.IsPercentageDefence
                        ? Mathf.RoundToInt(magnitude * defence * damage.DefenceCoefficient / 100.0f)
                        : magnitude - defence * damage.DefenceCoefficient;
                if (magnitude <= 0) {
                    continue;
                }
                
                modifiers.Add(new Modifier(magnitude, Modifier.Operation.Offset, this.TargetAttribute.Id));
            }
            
            return modifiers;
        }
    }
}
