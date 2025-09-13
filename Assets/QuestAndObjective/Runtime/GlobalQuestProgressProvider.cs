using System.Collections.Generic;

namespace QuestAndObjective.Runtime {
    public sealed class GlobalQuestProgressProvider : QuestProgressProvider {
        private Dictionary<string, int> QuestVariables { get; } = new Dictionary<string, int>();
        private HashSet<string> QuestFlags { get; } = new HashSet<string>();
        
        public override bool HasValue(string variableName, out int value) {
            return this.QuestVariables.TryGetValue(variableName, out value) || base.HasValue(variableName, out value);
        }

        public override bool HasFlag(string flagName) {
            return this.QuestFlags.Contains(flagName) || base.HasFlag(flagName);
        }

        public void SetVariable(string variableName, int value) {
            this.QuestVariables[variableName] = value;
        }

        public void UpdateVariable(string variableName, int delta) {
            this.QuestVariables[variableName] = this.QuestVariables.GetValueOrDefault(variableName, 0) + delta;
        }
        
        public void AddFlag(string flagName, bool value) {
            this.QuestFlags.Add(flagName);
        }
        
        public void RemoveFlag(string flagName) {
            this.QuestFlags.Remove(flagName);
        }
        
        public void ToggleFlag(string flagName) {
            if (!this.QuestFlags.Add(flagName)) {
                this.QuestFlags.Remove(flagName);
            }
        }
    }
}
