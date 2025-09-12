using System;

namespace QuestAndObjective.Runtime {
    public sealed class Quest {
        private QuestData Data { get; set; }
        private int CurrentStageIndex { get; set; }
        public QuestStage CurrentStage => this.Data.Stages[this.CurrentStageIndex];
        internal bool IsCompleted => this.CurrentStageIndex >= this.Data.Stages.Count;
        internal string Id => this.Data.Id;

        internal event Action<Objective> OnObjectiveCompleted; 

        internal Quest(QuestData data) {
            this.Data = data;
        }

        internal void Start() {
            foreach (QuestStage stage in this.Data.Stages) {
                stage.Begin();
                stage.OnProgressed += handleStageProgress;
            }

            return;
            void handleStageProgress(Objective completedObjective) {
                this.OnObjectiveCompleted?.Invoke(completedObjective);
            }
        }

        internal bool Advance<E>(E @event) where E : struct, IObjectiveStateUpdateEvent {
            this.CurrentStage.Advance(@event);
            if (this.CurrentStage.CompletionStatus != QuestStage.Status.Completed) {
                return false;
            }
            
            this.CurrentStageIndex += 1;
            return true;
        }

        internal void Complete() {
            this.OnObjectiveCompleted = null;
        }
    }
}
