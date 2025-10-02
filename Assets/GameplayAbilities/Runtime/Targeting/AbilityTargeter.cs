using System;
using System.Collections.Generic;
using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.Targeting {
    [DisallowMultipleComponent, RequireComponent(typeof(AbilitySystem)), RequireComponent(typeof(AttributeSet))]
    public sealed class AbilityTargeter : MonoBehaviour {
        public AttributeSet AttributeSet { get; private set; }
        public TargetingStrategy TargetingStrategy { get; set; }
        [field: SerializeField] private Transform Reticle { get; set; }
        [field: SerializeField, Tag] public List<string> AbilityTargets { get; set; } = new List<string>();
        public bool IsTargting => this.TargetingStrategy is not null;
        
        public Vector3 ProjectileOrigin => this.Reticle.position;

        private void Awake() {
            this.AttributeSet = this.GetComponent<AttributeSet>();
        }

        private void Update() {
            this.TargetingStrategy?.Update();
        }

        public void Cancel() {
            this.TargetingStrategy?.Cancel();
            this.TargetingStrategy = null;
        }

        public void Confirm() {
#if DEBUG
            Debug.Log("Confirm target");
#endif
            this.TargetingStrategy?.Confirm();
            this.TargetingStrategy = null;
        }
    }
}
