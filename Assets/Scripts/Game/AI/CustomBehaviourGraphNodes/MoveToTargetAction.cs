using System;
using Game.CharacterControls;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Game.AI.CustomBehaviourGraphNodes {
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Move To Target", story: "[Agent] moves to [Target]", category: "Action/Navigation",
        id: "10f500e14337530298c96976aed5a6b7"
    )]
    public partial class MoveToTargetAction : Action {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<Movement> MovementComponent;
        [SerializeReference] public BlackboardVariable<float> DistanceThreshold = new BlackboardVariable<float>(0.2f);

        protected override Status OnStart() {
            this.MovementComponent.Value.MoveTo(this.Target.Value.transform.position);
            return Status.Running;
        }

        protected override Status OnUpdate() {
            this.MovementComponent.Value.MoveTo(this.Target.Value.transform.position);
            if (!this.MovementComponent.Value.IsMoving) {
                return Status.Success;
            }

            Vector3 position = this.Agent.Value.transform.position;
            return Vector3.Distance(position, this.Target.Value.transform.position) <= this.DistanceThreshold
                    ? Status.Success
                    : Status.Running;
        }

        protected override void OnEnd() {
            this.MovementComponent.Value.Stop();
        }
    }
}
