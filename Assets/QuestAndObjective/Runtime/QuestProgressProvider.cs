using System.Collections.Generic;
using UnityEngine;

namespace QuestAndObjective.Runtime {
    public abstract class QuestProgressProvider : IQuestProgressProvider {
        private IQuestProgressProvider Next { get; set; }

        public virtual bool HasValue(string variableName, out int value) {
            value = 0;
            return this.Next?.HasValue(variableName, out value) ?? false;
        }

        public virtual bool HasFlag(string flagName) {
            return this.Next?.HasFlag(flagName) ?? false;       
        }

        public IQuestProgressProvider And(IQuestProgressProvider other) {
            this.Next = other;
            return this.Next;       
        }
    }
    
    public abstract class QuestProgressProvider<T> : QuestProgressProvider where T : MonoBehaviour {
        public T Source { get; }
        
        protected QuestProgressProvider(T source) {
            this.Source = source;
        }
    }
}
