using System;
using SaintsField;
using UnityEngine;

namespace QuestAndObjective.Runtime {
    public class VariableBasedObjective : Objective {
        protected enum Condition {
            GreaterThan,
            LessThan,
            EqualTo,
            NotEqualTo,
            GreaterThanOrEqualTo,
            LessThanOrEqualTo
        }
        
        protected bool IsReadonlyCondition { private get; set; }
        
        [field: SerializeField, ReadOnly(nameof(this.IsReadonlyCondition))] 
        protected string Variable { private get; set; }
        
        [field: SerializeField, ReadOnly(nameof(this.IsReadonlyCondition))] 
        protected Condition Predicate { private get; set; }
        
        [field: SerializeField] 
        protected int TargetValue { get; set; }
        
        [field: SerializeField] protected int InitialValue { private get; set; }
        
        private int CurrentValue { get; set; }

        protected internal override string Name {
            get {
                char verb = this.Predicate switch {
                    Condition.GreaterThan => '>',
                    Condition.LessThan => '<',
                    Condition.EqualTo => '=',
                    Condition.NotEqualTo => '≠',
                    Condition.GreaterThanOrEqualTo => '≥',
                    Condition.LessThanOrEqualTo => '≤',
                    var _ => '?'
                };
            
                return $"{this.Variable} {verb} {this.TargetValue}";
            }
        }

        public override void Initialise() {
            this.CurrentValue = this.InitialValue;
        }

        public override bool IsCompleted(QuestVariableContainer variables) {
            return this.Predicate switch {
                Condition.EqualTo => variables.GetIntValue(this.Variable) == this.TargetValue,
                Condition.GreaterThan => variables.GetIntValue(this.Variable) > this.TargetValue,
                Condition.GreaterThanOrEqualTo => variables.GetIntValue(this.Variable) >= this.TargetValue,
                Condition.LessThan => variables.GetIntValue(this.Variable) < this.TargetValue,
                Condition.LessThanOrEqualTo => variables.GetIntValue(this.Variable) <= this.TargetValue,
                Condition.NotEqualTo => variables.GetIntValue(this.Variable) != this.TargetValue,
                var _ => false
            };
        }

        public override bool Advance(QuestVariableContainer variables) {
            if (this.IsCompleted(variables)) {
                return false;
            }
            
            int distance = Math.Abs(this.TargetValue - this.CurrentValue);
            this.CurrentValue = variables.GetIntValue(this.Variable);
            return Math.Abs(this.TargetValue - this.CurrentValue) < distance;
        }
    }
}
