using System;
using GameplayAbilities.Runtime.Abilities;
using UnityEngine;
using Ability = GameplayAbilities.Runtime.Abilities.Ability;

namespace GameplayAbilities.Runtime.Targeting {
    [DisallowMultipleComponent, RequireComponent(typeof(AbilitySystem))]
    public sealed class AbilityTargeter : MonoBehaviour {
        public TargetingStrategy TargetingStrategy { get; set; }

        private void Update() {
            this.TargetingStrategy?.Update();
        }
    }
}
