using System;
using System.Collections.Generic;
using System.Linq;
using SaintsField;
using UnityEngine;

namespace QuestAndObjective.Runtime {
    public class VariableBasedObjective : Objective {
        protected enum Condition {
            GreaterThan,
            LessThan,
            Equal,
            NotEqual,
            GreaterThanOrEqual,
            LessThanOrEqual
        }
        
        protected bool IsReadonlyCondition { private get; set; }
        
        [field: SerializeField, ReadOnly(nameof(this.IsReadonlyCondition))] 
        protected string Variable { private get; set; }
        
        [field: SerializeField, ReadOnly(nameof(this.IsReadonlyCondition))] 
        protected Condition Predicate { private get; set; }
        
        [field: SerializeField, ReadOnly(nameof(this.IsReadonlyCondition))] 
        protected bool IncludesHistoricalValues { private get; set; } = true;
        
        [field: SerializeField] 
        protected int TargetValue { get; set; }
        
        [field: SerializeField] protected int InitialValue { private get; set; }
        
        private int CurrentValue { get; set; }

        protected internal override string Name {
            get {
                char verb = this.Predicate switch {
                    Condition.GreaterThan => '>',
                    Condition.LessThan => '<',
                    Condition.Equal => '=',
                    Condition.NotEqual => '≠',
                    Condition.GreaterThanOrEqual => '≥',
                    Condition.LessThanOrEqual => '≤',
                    var _ => '?'
                };
            
                return $"{this.Variable} {verb} {this.TargetValue}";
            }
        }

        public override void Initialise(IQuestProgressProvider provider) {
            if (this.IncludesHistoricalValues) {
                this.CurrentValue = provider.HasValue(this.Variable, out int value)
                        ? value + this.InitialValue
                        : this.InitialValue;
            } else {
                this.CurrentValue = this.InitialValue;
            }
        }

        public override bool IsCompleted(IQuestProgressProvider provider) {
            return this.Predicate switch {
                Condition.Equal => provider.HasValue(this.Variable, out int value) && value == this.TargetValue,
                Condition.NotEqual => provider.HasValue(this.Variable, out int value) && value != this.TargetValue,
                Condition.GreaterThan => provider.HasValue(this.Variable, out int value) && value > this.TargetValue,
                Condition.LessThan => provider.HasValue(this.Variable, out int value) && value < this.TargetValue,
                Condition.GreaterThanOrEqual => provider.HasValue(this.Variable, out int value) &&
                                                value >= this.TargetValue,
                Condition.LessThanOrEqual => provider.HasValue(this.Variable, out int value) &&
                                             value <= this.TargetValue,
                var _ => false
            };
        }

        public override bool Advance(IQuestProgressProvider provider) {
            if (this.IsCompleted(provider)) {
                return false;
            }

            if (provider.HasValue(this.Variable, out int value)) {
                return Math.Abs(value - this.CurrentValue) < Math.Abs(this.TargetValue - this.CurrentValue);
            }
            
            return false;
        }
    }
}
