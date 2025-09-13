using System.Collections.Generic;

namespace QuestAndObjective.Runtime {
    public sealed class QuestVariableContainer {
        private Dictionary<string, int> Variables { get; } = new Dictionary<string, int>();
        private Dictionary<string, bool> Flags { get; } = new Dictionary<string, bool>();
        
        public int GetIntValue(string variableName) {
            return this.Variables.GetValueOrDefault(variableName, 0);
        }
        
        public void SetIntValue(string variableName, int value) {
            this.Variables[variableName] = value;
        }
        
        public void UpdateIntValue(string variableName, int delta) {
            this.Variables[variableName] = this.GetIntValue(variableName) + delta;       
        }

        public bool HasFlag(string flagName) {
            return this.Flags.GetValueOrDefault(flagName, false);
        }
        
        public void SetFlag(string flagName, bool value) {
            this.Flags[flagName] = value;       
        }
        
        public void ToggleFlag(string flagName) {
            this.Flags[flagName] = !this.HasFlag(flagName);
        }
    }
}
