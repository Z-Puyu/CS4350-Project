using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Game.AI.CustomBehaviourGraphNodes {
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Reset Target", story: "Reset [Target] at [Position]", category: "Action/Ai Behaviour",
        id: "80c31b55e0f6ba00c3ec5ff3aae56989"
    )]
    public partial class ResetTargetAction : Action {
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<Vector3> Position;

        protected override Status OnStart() {
            this.Target.SetValueWithoutNotify(null);
            this.Position.SetValueWithoutNotify(Vector3.zero);
            return Status.Success;
        }
    }
}
