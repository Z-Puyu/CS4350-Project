using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace QuestAndObjective.Runtime {
    public sealed class AndObjective : Objective {
        [field: SerializeReference, ReferencePicker, RichLabel(nameof(this.ObjectiveLabels), true)]  
        private List<Objective> SubObjectives { get; set; } = new List<Objective>();

        protected internal override string Name => "All of the following:";
        
        private string ObjectiveLabels(Objective obj, int index) => obj is null ? $"{index + 1}." : $"{index + 1}. {obj.Name}";

        public override void Initialise(IQuestProgressProvider provider) {
            this.SubObjectives.ForEach(objective => objective.Initialise(provider));
        }
        
        public override bool IsCompleted(IQuestProgressProvider provider) {
            return this.SubObjectives.TrueForAll(objective => objective.IsCompleted(provider));
        }
        
        public override bool Advance(IQuestProgressProvider provider) {
            return this.SubObjectives.Exists(objective => objective.Advance(provider));
        }
    }
}
