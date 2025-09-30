using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using DataStructuresForUnity.Runtime.Trie;
using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.HealthSystem;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using UnityEngine;
using Weapons.Runtime;

namespace WeaponsSystem.DamageHandling {
    [DisallowMultipleComponent]
    public sealed class DamageHandler : MonoBehaviour {
        [field: SerializeReference] private IAbility DamageAbility { get; set; } 
        [field: SerializeField, Required] private AttributeSet AttributeSet { get; set; }
        [field: SerializeField] private Health Health { get; set; }
        
        public void HandleDamage(Damage damage) {
            int health = this.Health.Value;
            this.DamageAbility.Execute(new ReadonlyAttributes(damage), this.AttributeSet);
            int damageMagnitude = health - this.Health.Value;
#if DEBUG
            OnScreenDebugger.Log($"Caused {damageMagnitude} damage on {this.gameObject.name}");
#endif
        }
    }
}
