using System;
using Game.CharacterControls;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Random = UnityEngine.Random;

namespace Game.AI.CustomBehaviourGraphNodes {
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Wander", story: "[Agent] wanders for at most a distance of [MaxWanderDistance]", 
        category: "Action/Ai Behaviour", id: "9c1f20c6e582807b65da62def2c78ad4"
    )]
    public partial class WanderAction : Action {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<float> MaxWanderDistance;
        [SerializeReference] public BlackboardVariable<Movement> MovementComponent;
        private Vector3 StartPosition { get; set; }
        private float WanderDistance { get; set; }

        protected override Status OnStart() {
            this.StartPosition = this.Agent.Value.transform.position;
            this.MovementComponent.Value.MoveTo(Random.insideUnitCircle.normalized);
            this.WanderDistance = Random.Range(1f, this.MaxWanderDistance.Value);
            return Status.Running;
        }

        protected override Status OnUpdate() {
            if (!this.MovementComponent.Value.IsMoving) {
                return Status.Success;
            }
            
            Vector3 position = this.Agent.Value.transform.position;
            float distance = Vector3.Distance(position, this.StartPosition);
            return distance >= this.WanderDistance ? Status.Success : Status.Running;
        }

        protected override void OnEnd() {
            this.MovementComponent.Value.Stop();
        }
    }
}
