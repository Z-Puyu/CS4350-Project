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

        [field: SerializeReference, ReferencePicker]
        private List<Objective> Objectives { get; set; } = new List<Objective>();

        internal event Action<Objective> OnProgressed; 
        
        internal void Begin() {
            this.CompletionStatus = Status.Ongoing;
        }
        
        internal bool Advance<E>(E @event) where E : struct, IObjectiveStateUpdateEvent {
            foreach (Objective objective in this.Objectives.Where(objective => objective.Advance(@event))) {
                this.OnProgressed?.Invoke(objective);
            }

            if (!this.Objectives.TrueForAll(objective => objective.IsCompleted)) {
                return false;
            }

            this.OnProgressed = null;
            this.CompletionStatus = Status.Completed;
            return true;
        }
    }
}
