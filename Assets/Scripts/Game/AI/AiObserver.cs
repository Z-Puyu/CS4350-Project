using System.Collections.Generic;
using SaintsField;
using Unity.Behavior;
using Unity.Behavior.GraphFramework;
using UnityEngine;

namespace Game.AI {
    [DisallowMultipleComponent, RequireComponent(typeof(BehaviorGraphAgent))]
    public class AiObserver : MonoBehaviour {
        private BehaviorGraphAgent Agent { get; set; }
        
        [field: SerializeField, Dropdown(nameof(this.GetBlackboardVariables))]
        private SerializableGUID TargetVariable { get; set; }

        private void Awake() {
            this.Agent = this.GetComponent<BehaviorGraphAgent>();
        }
        
        private DropdownList<SerializableGUID> GetBlackboardVariables() {
            DropdownList<SerializableGUID> variables = new DropdownList<SerializableGUID>();
            List<BlackboardVariable> blackboard = this.GetComponent<BehaviorGraphAgent>()
                                                      .BlackboardReference.Blackboard.Variables;
            foreach (BlackboardVariable variable in blackboard) {
                variables.Add(variable.Name, variable.GUID);
            }
            
            return variables;
        }

        public void LockTarget(GameObject target) {
            this.Agent.SetVariableValue(this.TargetVariable, target);
        }

        public void LoseTarget(GameObject target) {
            if (this.Agent.GetVariable(this.TargetVariable, out BlackboardVariable var) &&
                object.ReferenceEquals(var.ObjectValue, target)) {
                this.Agent.SetVariableValue<GameObject>(this.TargetVariable, null);
            }
        }
    }
}
