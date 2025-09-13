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

        /// <summary>
        /// Advances a quest.
        /// </summary>
        /// <param name="variables">The quest variables</param>
        /// <returns><c>true</c> if the quest is completed, <c>false</c> otherwise</returns>
        internal bool Advance(QuestVariableContainer variables) {
            do {
                this.CurrentStage.Advance(variables);
                if (this.CurrentStage.CompletionStatus != QuestStage.Status.Completed) {
                    return false;
                }
                
                this.CurrentStageIndex += 1;
                if (this.CurrentStageIndex < this.Data.Stages.Count) {
                    this.CurrentStage.Begin();
                }
            } while (this.CurrentStageIndex < this.Data.Stages.Count);
            
            return true;
        }

        internal void Complete() {
            this.OnObjectiveCompleted = null;
        }
    }
}
