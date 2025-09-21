using System;
using Game.CharacterControls;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Game.AI.CustomBehaviourGraphNodes {
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Move To Location", story: "[Agent] moves to [Location]", category: "Action/Navigation",
        id: "76caaa6d9c89c6855f7adc9fba919731"
    )]
    public partial class MoveToLocationAction : Action {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<Vector3> Location;
        [SerializeReference] public BlackboardVariable<Movement> MovementComponent;
        [SerializeReference] public BlackboardVariable<float> DistanceThreshold = new BlackboardVariable<float>(0.2f);

        protected override Status OnStart() {
            this.MovementComponent.Value.MoveTo(this.Location);
            return Status.Running;
        }

        protected override Status OnUpdate() {
            if (!this.MovementComponent.Value.IsMoving) {
                return Status.Success;
            }

            Vector3 position = this.Agent.Value.transform.position;
            return Vector3.Distance(position, this.Location) <= this.DistanceThreshold
                    ? Status.Success
                    : Status.Running;
        }

        protected override void OnEnd() {
            this.MovementComponent.Value.Stop();
        }
    }
}
