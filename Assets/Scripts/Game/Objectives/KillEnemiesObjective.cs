using QuestAndObjective.Runtime;

namespace Game.Objectives {
    public sealed class KillEnemiesObjective : VariableBasedObjective {
        protected override string Name => this.TargetValue > 1 ? $"Kill {this.TargetValue} enemies" : "Kill any enemy";
        
        public KillEnemiesObjective() {
            this.IsReadonlyCondition = true;
            this.Predicate = Condition.GreaterThanOrEqual;
            this.IncludesHistoricalValues = false;
            this.Variable = "kill_count";
        }
    }
}
