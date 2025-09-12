using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QuestAndObjective.Runtime {
    [DisallowMultipleComponent]
    public sealed class QuestLog : MonoBehaviour, IEnumerable<Quest> {
        private List<Quest> Quests { get; set; } = new List<Quest>();
        
        public void Begin(string id) {
            if (this.Quests.Any(q => q.Id == id)) {
                Debug.LogWarning($"Cannot begin quest {id} twice", this);
            } else if (!QuestDatabase.TryCreate(id, out Quest quest)) {
                Debug.LogWarning($"Quest {id} does not exist.", this);
            } else {
                this.Quests.Add(quest);
            }
        }

        public void Progress<E>(E @event) where E : struct, IObjectiveStateUpdateEvent {
            List<Quest> completed = new List<Quest>();
            foreach (Quest quest in this.Quests) {
                quest.Update(@event);
                if (quest.IsCompleted) {
                    completed.Add(quest);
                }
            }
            
            completed.ForEach(this.Finish);
        }

        public void Finish(Quest quest) {
            if (!this.Quests.Remove(quest) || !QuestDatabase.Remove(quest.Id)) {
                Debug.LogWarning($"Quest {quest.Id} does not exist.", this);
            }
        }
        
        public void Finish(string id) {
            int size = this.Quests.Count;
            this.Quests.RemoveAll(quest => quest.Id == id);
            if (size == this.Quests.Count || !QuestDatabase.Remove(id)) {
                Debug.LogWarning($"Quest {id} does not exist.", this);
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
