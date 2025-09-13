using System;
using SaintsField;
using UnityEngine;

namespace QuestAndObjective.Runtime {
    public class FlagBasedObjective : Objective {
        protected bool IsReadonlyCondition { private get; set; }
        
        [field: SerializeField, ReadOnly(nameof(this.IsReadonlyCondition))] 
        protected string Flag { private get; set; }

        [field: SerializeField] protected bool WantsFlagToBePresent { private get; set; } = true;
        
        private bool CurrentValue { get; set; }

        protected internal override string Name => $"Flag {this.Flag} {(this.WantsFlagToBePresent ? "is" : "is not")} present";

        public override void Initialise() {
            this.CurrentValue = !this.WantsFlagToBePresent;
        }

        public override bool IsCompleted(QuestVariableContainer variables) {
            return this.CurrentValue == this.WantsFlagToBePresent;
        }

        public override bool Advance(QuestVariableContainer variables) {
            if (this.IsCompleted(variables)) {
                return false;
            }
            
            this.CurrentValue = variables.HasFlag(this.Flag);
            return this.CurrentValue == this.WantsFlagToBePresent;
        }
    }
}
