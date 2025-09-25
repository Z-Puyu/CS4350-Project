using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.GameplayEffects {
    public sealed class ModifierEffectData : GameplayEffectData {
        [field: SerializeField, Table]
        private List<ModifierData> Modifiers { get; set; } = new List<ModifierData>();
        
        public override IEnumerable<Modifier> Run(AttributeSet target, GameplayEffectExecutionArgs args) {
            return this.Modifiers.Select(modifier => modifier.CreateModifier(target, args) * args.Level);
        }

        public override DropdownList<string> GetDataLabels() {
            DropdownList<string> labels = new DropdownList<string>();
            foreach (ModifierData modifier in this.Modifiers.Where(m => m.AllowSetByCaller)) {
                labels.Add(modifier.TargetAttribute, modifier.Label);
            }

            return labels;
        }

        protected override string GenerateSortKey() {
            StringBuilder sb = new StringBuilder(base.GenerateSortKey()).Append("-Modifiers:");
            foreach (ModifierData modifier in this.Modifiers) {
                sb.Append(modifier.SortKey);
            }

            return sb.ToString();
        }
    }
}
