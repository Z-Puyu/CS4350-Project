using System;
using System.Collections.Generic;
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
        
        internal void Update<E>(E @event) where E : struct, IObjectiveStateUpdateEvent {
            this.Objectives.ForEach(objective => objective.Update(@event));
            if (this.Objectives.TrueForAll(objective => objective.IsCompleted)) {
                this.CompletionStatus = Status.Completed;
            }
        }
    }
}
