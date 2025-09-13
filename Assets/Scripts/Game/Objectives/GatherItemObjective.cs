using System;
using ModularItemsAndInventory.Runtime.Items;
using QuestAndObjective.Runtime;
using SaintsField;
using UnityEngine;

namespace Game.Objectives {
    public class GatherItemObjective : VariableBasedObjective {
        [field: SerializeField, Required, OnValueChanged(nameof(this.OnValidate))] 
        private ItemData Item { get; set; }

        protected override string Name => this.TargetValue > 1
                ? $"Has at least {this.TargetValue} {this.Item.Name} items"
                : $"Has any {this.Item.Name} item";

        public GatherItemObjective() {
            this.IsReadonlyCondition = true;
            this.Predicate = Condition.GreaterThanOrEqualTo;
        }

        private void OnValidate() {
            this.Variable = $"{this.Item.Id}_count";
        }
    }
}
