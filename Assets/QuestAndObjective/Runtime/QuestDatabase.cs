using System.Collections.Generic;
using DataStructuresForUnity.Runtime.GeneralUtils;
using SaintsField;
using UnityEngine;

namespace QuestAndObjective.Runtime {
    [DisallowMultipleComponent]
    public sealed class QuestDatabase : Singleton<QuestDatabase> {
        private Dictionary<string, QuestData> Quests { get; set; } =
            new Dictionary<string, QuestData>();

        private Dictionary<string, Quest> RuntimeQuests { get; set; } =
            new Dictionary<string, Quest>();
        
        [field: SerializeField, ResourceFolder] private string QuestDataFolder { get; set; }
        
        protected override void Awake() {
            base.Awake();
            foreach (QuestData data in Resources.LoadAll<QuestData>(this.QuestDataFolder)) {
                if (this.Quests.TryGetValue(data.Id, out QuestData existing)) {
                    Debug.LogError($"Duplicate quest ID {data.Id} between {data.name} and {existing.name}", this);
                    continue;
                }
                
                this.Quests.Add(data.Id, data);
            }
        }

        public static bool TryGet(string id, out QuestData quest) {
            return Singleton<QuestDatabase>.Instance.Quests.TryGetValue(id, out quest);
        }
        
        public static Quest Get(string id) {
            if (Singleton<QuestDatabase>.Instance.RuntimeQuests.TryGetValue(id, out Quest quest)) {
                return quest;
            }

            if (QuestDatabase.TryGet(id, out QuestData data)) {
                quest = new Quest(data);
                Singleton<QuestDatabase>.Instance.RuntimeQuests.Add(id, quest);    
                return quest;
            } 
            
            Debug.LogError($"Quest {id} not found", Singleton<QuestDatabase>.Instance);
            return null;
        }

        public static bool Remove(string id) {
            return Singleton<QuestDatabase>.Instance.RuntimeQuests.Remove(id);
        }

        public static bool Contains(string id) {
            return Singleton<QuestDatabase>.Instance.Quests.ContainsKey(id);
        }

        public static bool TryCreate(string id, out Quest quest) {
            if (Singleton<QuestDatabase>.Instance.RuntimeQuests.TryGetValue(id, out quest)) {
                return false;
            }
            
            if (QuestDatabase.TryGet(id, out QuestData data)) {
                quest = new Quest(data);
                Singleton<QuestDatabase>.Instance.RuntimeQuests.Add(id, quest);
                return true;
            }
            
            Debug.LogError($"Quest {id} not found", Singleton<QuestDatabase>.Instance);
            return false;
        }
    }
}
