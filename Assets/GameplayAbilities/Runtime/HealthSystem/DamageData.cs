using System;
using System.Text;
using DataStructuresForUnity.Runtime.Utilities;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.HealthSystem {
    [Serializable]
    public class DamageData : ModifierData {
        [field: SerializeField, TableColumn("Defended by"), TreeDropdown(nameof(this.AttributeOptions))] 
        private string DefenceAttribute { get; set; }
        
        [field: SerializeField, TableColumn("Defended by")] 
        private bool IsPercentageDefence { get; set; }
        
        [field: SerializeField, TableColumn("Defended by")]
        private int DefenceCoefficient { get; set; }

        protected override string GenerateSortKey() {
            return new StringBuilder(base.GenerateSortKey())
                   .Append($"-DefendedBy:{this.DefenceAttribute}")
                   .Append($"Coefficient:{this.DefenceCoefficient}")
                   .ToString();
        }

        public override Modifier CreateModifier(IAttributeReader target) {
            Modifier modifier = base.CreateModifier(target);
            int defence = target.GetCurrent(this.DefenceAttribute) * this.DefenceCoefficient;
            if (this.IsPercentageDefence) {
                modifier *= Math.Min(0, 100 - defence) / 100.0f;
            } else {
                modifier -= defence;
            }
            
            return modifier;
        }

        public override Modifier CreateModifier(IDataReader<string, int> target) {
            Modifier modifier = base.CreateModifier(target);
            int defence = (target.HasValue(this.DefenceAttribute, out int value) ? value : 0) * this.DefenceCoefficient;
            if (this.IsPercentageDefence) {
                modifier *= Math.Min(0, 100 - defence) / 100.0f;
            } else {
                modifier -= defence;
            }
            
            return modifier;
        }

        public override Modifier CreateModifier(IAttributeReader target, IDataReader<string, int> source) {
            Modifier modifier = base.CreateModifier(target, source);
            int defence = (target.HasValue(this.DefenceAttribute, out int value) ? value : 0) * this.DefenceCoefficient;
            if (this.IsPercentageDefence) {
                modifier *= Math.Min(0, 100 - defence) / 100.0f;
            } else {
                modifier -= defence;
            }
            
            return modifier;
        }
    }
}
