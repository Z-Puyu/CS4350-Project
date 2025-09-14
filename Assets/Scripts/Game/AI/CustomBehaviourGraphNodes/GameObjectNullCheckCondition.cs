using System;
using Unity.Behavior;
using UnityEngine;

namespace Game.AI.CustomBehaviourGraphNodes {
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(
        name: "Game Object Null Check", story: "[GameObject] [Verb] null", category: "Variable Conditions",
        id: "f436db81589e34a8a8b36ee209b79fce"
    )]
    public partial class GameObjectNullCheckCondition : Condition {
        [SerializeReference] public BlackboardVariable<GameObject> GameObject;

        [Comparison(comparisonType: ComparisonType.Boolean)]
        [SerializeReference]
        public BlackboardVariable<ConditionOperator> Verb;

        public override bool IsTrue() {
            return this.Verb == ConditionOperator.Equal ? !this.GameObject.Value : this.GameObject.Value;
        }
    }
}
