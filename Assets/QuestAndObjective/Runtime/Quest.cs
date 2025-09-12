using System;

namespace QuestAndObjective.Runtime {
    public sealed class Quest {
        private QuestData Data { get; set; }
        private int CurrentStageIndex { get; set; }
        public QuestStage CurrentStage => this.Data.Stages[this.CurrentStageIndex];
        internal bool IsCompleted => this.CurrentStageIndex >= this.Data.Stages.Count;
        internal string Id => this.Data.Id;

        internal Quest(QuestData data) {
            this.Data = data;
        }

        internal void Advance() {
            this.CurrentStageIndex += 1;
        }

        internal void Update<E>(E @event) where E : struct, IObjectiveStateUpdateEvent {
            this.CurrentStage.Update(@event);
            if (this.CurrentStage.CompletionStatus == QuestStage.Status.Completed) {
                this.Advance();
            }
        }
    }
}
