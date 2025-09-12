using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace QuestAndObjective.Runtime {
    [DisallowMultipleComponent]
    public sealed class QuestLog : MonoBehaviour, IEnumerable<Quest> {
        private List<Quest> Quests { get; set; } = new List<Quest>();

        public event UnityAction<Objective> OnObjectiveAchieved;
        public event UnityAction<(QuestStage from, QuestStage to)> OnQuestAdvanced;
        public event UnityAction<Quest> OnQuestCompleted;
        public event UnityAction<Quest> OnQuestStarted;
        
        public void Begin(string id) {
            if (this.Quests.Any(q => q.Id == id)) {
                Debug.LogWarning($"Cannot begin quest {id} twice", this);
            } else if (!QuestDatabase.TryCreate(id, out Quest quest)) {
                Debug.LogWarning($"Quest {id} does not exist.", this);
            } else {
                this.Quests.Add(quest);
                quest.Start();
                quest.OnObjectiveCompleted += handleCompletedObjective;
                this.OnQuestStarted?.Invoke(quest);
            }

            return;
            void handleCompletedObjective(Objective completedObjective) {
                this.OnObjectiveAchieved?.Invoke(completedObjective);
            }
        }

        public void Progress<E>(E @event) where E : struct, IObjectiveStateUpdateEvent {
            List<Quest> completed = new List<Quest>();
            foreach (Quest quest in this.Quests) {
                QuestStage prev = quest.CurrentStage;
                if (quest.Advance(@event)) {
                    this.OnQuestAdvanced?.Invoke((prev, quest.CurrentStage));
                }
                
                if (quest.IsCompleted) {
                    completed.Add(quest);
                }
            }
            
            completed.ForEach(this.Finish);
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
