using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Game.AI.CustomBehaviourGraphNodes {
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Search for a Target", story: "[Agent] searches for a [Target] with [Sensor]",
        category: "Action/Ai Behaviour", id: "9871a09bb30ef5ecd03f1899e84c5120"
    )]
    public partial class SearchForATargetAction : Action {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;

        [SerializeReference, Tooltip("[Out Value] If a target is found, the field is assigned with it.")]
        public BlackboardVariable<GameObject> Target;
        
        [SerializeReference, Tooltip("[Out Value] If a target is found, the field is assigned with its position.")]
        public BlackboardVariable<Vector3> TargetPosition;

        [SerializeReference] public BlackboardVariable<Sensor> Sensor;
        [SerializeReference] public BlackboardVariable<bool> RemenberPositionOnly;

        protected override Status OnStart() {
            return Status.Running;
        }

        protected override Status OnUpdate() {
            if (!this.Sensor.Value.LastDetectedTarget) {
                return Status.Running;
            }

            if (!this.RemenberPositionOnly.Value) {
                this.Target.Value = this.Sensor.Value.LastDetectedTarget;
            }
            
            this.TargetPosition.Value = this.Sensor.Value.LastDetectedTarget.transform.position;
            return Status.Success;
        }
    }
}
