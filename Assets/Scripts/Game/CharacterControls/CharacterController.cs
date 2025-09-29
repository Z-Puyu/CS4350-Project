using Common;
using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.HealthSystem;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.Events;

namespace Game.CharacterControls {
    [DisallowMultipleComponent]
    public abstract class CharacterController : MonoBehaviour {
        [field: SerializeField, Required] protected AbilitySystem AbilitySystem { get; private set; }
        [field: SerializeField, Required] protected AttributeSet AttributeSet { get; private set; }
        [field: SerializeField, Required] protected Health Health { get; private set; }
        [field: SerializeField, Required] protected DamageHandler DamageHandler { get; private set; }

        public event UnityAction OnDeath;
        
        protected abstract void ConfigureAttributeSet();
        
        protected virtual void Start() {
            this.ConfigureAttributeSet();
        }

        [Button]
        public virtual void HandleDeath() {
            this.OnDeath?.Invoke();
        }

        /// <summary>
        /// Destroy the character. Call this after the death animation is complete.
        /// </summary>
        public void Bury() {
            Object.Destroy(this.gameObject);
        }
        
        public void Say(string message) {
            OnScreenDebugger.Log($"{this.gameObject.name}: {message}");
        }
    }
}
