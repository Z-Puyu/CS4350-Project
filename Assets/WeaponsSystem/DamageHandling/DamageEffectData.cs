using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.DamageHandling {
    public sealed class DamageEffectData : GameplayEffectData {
        [field: SerializeField, Table]
        private List<DamageType> DamageTypes { get; set; } = new List<DamageType>();

        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))] 
        private string TargetAttribute { get; set; }
        
        private AdvancedDropdownList<string> AttributeOptions => this.GetAttributeOptions();

        public override DropdownList<string> GetDataLabels() {
            DropdownList<string> labels = new DropdownList<string>();
            foreach (DamageType damage in this.DamageTypes) {
                labels.Add(damage.DamageAttribute, damage.DamageAttribute);
            }
            
            return labels;
        }

        public override IEnumerable<Modifier> Run(AttributeSet target, GameplayEffectExecutionArgs args) {
            List<Modifier> modifiers = new List<Modifier>();
            int totalDamage = 0;
            foreach (DamageType damage in this.DamageTypes) {
                if (!args.HasData(damage.DamageAttribute, out int magnitude) || magnitude == 0) {
                    continue;
                }
                
                int defence = target.GetCurrent(damage.DefenceAttribute);
                magnitude = damage.IsPercentageDefence
                        ? Mathf.RoundToInt(magnitude * defence * damage.DefenceCoefficient / 100.0f)
                        : magnitude - defence * damage.DefenceCoefficient;
                if (magnitude <= 0) {
                    continue;
                }
                
                totalDamage += magnitude;
                modifiers.Add(new Modifier(-magnitude, Modifier.Operation.Offset, this.TargetAttribute));
            }
            
            return modifiers;
        }
    }
}
