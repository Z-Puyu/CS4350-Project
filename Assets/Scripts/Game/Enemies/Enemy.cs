using System;
using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.HealthSystem;
using ModularItemsAndInventory.Runtime.LootContainers;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Enemies {
    [DisallowMultipleComponent]
    public class Enemy : MonoBehaviour {
        public static event UnityAction<Enemy> OnDeath; 
        
        [field: SerializeField] private EnemyData Data { get; set; }
        [field: SerializeField] private Animator Animator { get; set; }
        [field: SerializeField, Required] private AttributeSet AttributeSet { get; set; }
        [field: SerializeField, Required] private LootContainer LootContainer { get; set; }
        [field: SerializeField, Required] private Health Health { get; set; }
        
        private void Start() {
            this.Animator.runtimeAnimatorController = this.Data.Animations;
            this.AttributeSet.Initialise(this.Data.Attributes);
            this.LootContainer.Use(this.Data.LootTable);
            this.Health.OnZeroHealth += this.Die;
        }

        private void Die() {
            Enemy.OnDeath?.Invoke(this);
        }
    }
}
