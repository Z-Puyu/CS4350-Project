using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using WeaponsSystem.Runtime.Combat;
//using WeaponsSystem.DamageHandling;
using Action = Unity.Behavior.Action;

namespace Game.AI.CustomBehaviourGraphNodes {
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Attack Target", story: "[Combatant] attacks [Target]", category: "Action/Ai Behaviour",
        id: "e6cc4af882139afc1cf572fa6298b821"
    )]
    public partial class AttackTargetAction : Action {
        [SerializeReference] public BlackboardVariable<Combatant> Combatant;
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        protected override Status OnStart() {
            this.Combatant.Value.StartAttack();
            return Status.Success;
        }
    }
}

