using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace QuestAndObjective.Runtime {
    [DisallowMultipleComponent]
    public sealed class QuestLog : MonoBehaviour, IEnumerable<Quest> {
        private List<Quest> Quests { get; set; } = new List<Quest>();
        private List<Quest> JustCompletedQuests { get; set; } = new List<Quest>();
        public QuestVariableContainer Variables { get; } = new QuestVariableContainer();
        private IQuestProgressProvider QuestProgressProvider { get; set; }

        public event UnityAction<Objective> OnObjectiveAchieved;
        public event UnityAction<(QuestStage from, QuestStage to)> OnQuestAdvanced;
        public event UnityAction<Quest> OnQuestCompleted;
        public event UnityAction<Quest> OnQuestStarted;
        
        public QuestLog WithQuestProgressProvider(IQuestProgressProvider provider) {
            this.QuestProgressProvider = provider;
            return this;
        }
        
        public void Begin(string id) {
            if (this.Quests.Any(q => q.Id == id)) {
                Debug.LogWarning($"Cannot begin quest {id} twice", this);
            } else if (!QuestDatabase.TryCreate(id, out Quest quest)) {
                Debug.LogWarning($"Quest {id} does not exist.", this);
            } else {
                this.Quests.Add(quest);
                quest.Start(this.QuestProgressProvider);
                quest.OnObjectiveCompleted += handleCompletedObjective;
                this.OnQuestStarted?.Invoke(quest);
                this.UpdateQuest(quest);
                if (quest.IsCompleted) {
                    this.Finish(quest);
                }
            }

            return; 
            
            void handleCompletedObjective(Objective completedObjective) {
                this.OnObjectiveAchieved?.Invoke(completedObjective);
            }
        }
        
        private void UpdateQuest(Quest quest) {
            QuestStage prev = quest.CurrentStage;
            if (quest.Advance(this.QuestProgressProvider)) {
                this.OnQuestAdvanced?.Invoke((prev, quest.CurrentStage));
            }
        }

        private void UpdateAllQuests() {
            foreach (Quest quest in this.Quests) {
                this.UpdateQuest(quest);
                if (quest.IsCompleted) {
                    this.JustCompletedQuests.Add(quest);
                }
            }

            this.JustCompletedQuests.ForEach(this.Finish);
            this.JustCompletedQuests.Clear();
        }

        public void Progress(string questVariable, int delta) {
            this.Variables.UpdateIntValue(questVariable, delta);
            this.UpdateAllQuests();
        }

        public void Finish(Quest quest) {
            if (!this.Quests.Remove(quest) || !QuestDatabase.Remove(quest.Id)) {
                Debug.LogWarning($"Quest {quest.Id} does not exist.", this);
            } else {
                this.OnQuestCompleted?.Invoke(quest);
                quest.Complete();
            }
        }
        
        public void Finish(string id) {
            if (!QuestDatabase.Remove(id, out Quest quest) || !this.Quests.Remove(quest)) {
                Debug.LogWarning($"Quest {id} does not exist.", this);
            } else {
                this.OnQuestCompleted?.Invoke(quest);
                quest.Complete();
            }
        }

        public IEnumerator<Quest> GetEnumerator() {
            return this.Quests.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
