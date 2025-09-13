using System;
using System.Collections.Generic;
using System.Linq;
using SaintsField;
using UnityEngine;

namespace QuestAndObjective.Runtime {
    [Serializable]
    public class QuestStage {
        public enum Status {
            NotStarted,
            Ongoing,
            Completed,
            Failed
        }

        internal Status CompletionStatus { get; private set; } = Status.NotStarted;

        [field: SerializeReference, ReferencePicker, RichLabel(nameof(this.ObjectiveLabels), true)]
        private List<Objective> Objectives { get; set; } = new List<Objective>();

        internal event Action<Objective> OnProgressed; 
        
        private string ObjectiveLabels(Objective obj, int index) => obj is null ? $"{index + 1}." : $"{index + 1}. {obj.Name}";
        
        internal void Begin() {
            this.CompletionStatus = Status.Ongoing;
            this.Objectives.ForEach(objective => objective.Initialise());
        }
        
        internal bool Advance(QuestVariableContainer variables) {
            foreach (Objective objective in this.Objectives.Where(objective => objective.Advance(variables))) {
                this.OnProgressed?.Invoke(objective);
            }

            if (!this.Objectives.TrueForAll(objective => objective.IsCompleted(variables))) {
                return false;
            }

            this.OnProgressed = null;
            this.CompletionStatus = Status.Completed;
            return true;
        }
    }
}
