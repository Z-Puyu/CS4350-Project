using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MushroomJellyfishAttack", story: "[Agent] enters attack phase [attackPhase] and attacks [attacks]", category: "Action", id: "f4358c0b49b0ece9b351b1d832b9671a")]
public partial class MushroomJellyfishAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<bool> AttackPhase;
    [SerializeReference] public BlackboardVariable<bool> Attacks;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

