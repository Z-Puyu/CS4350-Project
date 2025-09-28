using Common;
using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using GameplayAbilities.Runtime.HealthSystem;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;
using WeaponsSystem.DamageHandling;

namespace Game.CharacterControls {
    [DisallowMultipleComponent]
    public abstract class CharacterController : MonoBehaviour {
        [field: SerializeField, Required] protected AbilitySystem AbilitySystem { get; private set; }
        [field: SerializeField, Required] protected AttributeSet AttributeSet { get; private set; }
        [field: SerializeField, Required] protected Health Health { get; private set; }
        
        protected abstract void ConfigureAttributeSet();
        
        protected virtual void Start() {
            this.ConfigureAttributeSet();
            foreach (HitBox2D hitbox in this.GetComponentsInChildren<HitBox2D>()) {
                hitbox.OnHit += this.HandleDamage;
            }
        }
        
        protected void HandleDamage(Damage damage) {
            GameObject source = damage.Instigator;
            AbilitySystem instigator = source.GetComponentInChildren<AbilitySystem>();
            if (!instigator) {
                Debug.LogError($"{source.name} must have an Ability System to attack {this.gameObject.name}!", source);
            } else {
                this.Say($"{source.name} damaged {this.gameObject.name}!");
                GameplayEffectExecutionArgs args = instigator.CreateEffectExecutionArgs()
                                                             .WithUserData(damage.Data)
                                                             .Build();
                instigator.Use("ability:attack", this.AbilitySystem, args);
            }
        }

        public abstract void HandleDeath();

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
