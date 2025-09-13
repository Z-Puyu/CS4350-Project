using Game.Enemies;
using QuestAndObjective.Runtime;
using SaintsField;
using UnityEngine;

namespace Game.Objectives {
    public sealed class KillSpecificEnemiesObjective : VariableBasedObjective {
        [field: SerializeField, Required, OnValueChanged(nameof(this.OnValidate))]
        private EnemyData Enemy { get; set; }

        protected override string Name => this.TargetValue > 1
                ? $"Kill {this.TargetValue} {this.Enemy.Name} enemies"
                : $"Kill any {this.Enemy.Name}";
        
        public KillSpecificEnemiesObjective() {
            this.IsReadonlyCondition = true;
            this.Predicate = Condition.GreaterThanOrEqual;
            this.IncludesHistoricalValues = false;
        }

        private void OnValidate() {
            this.Variable = $"{this.Enemy.Id}:kill_count";
        }
    }
}
