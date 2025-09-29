using GameplayAbilities.Runtime.Abilities;
using UnityEngine;

namespace GameplayAbilities.Runtime.Targeting {
    [DisallowMultipleComponent, RequireComponent(typeof(AbilitySystem))]
    public sealed class AbilityTargeter : MonoBehaviour {
        public TargetingStrategy TargetingStrategy { get; set; }

        private void Update() {
            this.TargetingStrategy?.Update();
        }
    }
}
