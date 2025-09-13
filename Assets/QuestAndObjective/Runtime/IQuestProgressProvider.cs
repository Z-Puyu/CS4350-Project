namespace QuestAndObjective.Runtime {
    public interface IQuestProgressProvider {
        public bool HasValue(string variableName, out int value);
        public bool HasFlag(string flagName);
        public IQuestProgressProvider And(IQuestProgressProvider other);
    }
}
