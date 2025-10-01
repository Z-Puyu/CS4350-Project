using Events;
using Game.CharacterControls;
using ModularItemsAndInventory.Runtime.LootContainers;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Enemies {
    public class Enemy : GameCharacterController {
        public static event UnityAction<EnemyDeathEvent> OnDeath; 
        
        [field: SerializeField] private EnemyData Data { get; set; }
        [field: SerializeField, Required] private Animator Animator { get; set; }
        [field: SerializeField, Required] private CrossObjectEventWithDataSO broadcastEnemyDiedData;
        [field: SerializeField, Required] private LootContainer LootContainer { get; set; }
        private GameObject LastAttacker { get; set; }

        protected override void ConfigureAttributeSet() {
            this.AttributeSet.Initialise(this.Data.Attributes);
        }

        protected override void Start() {
            if (!this.Data) {
                return;
            }
            
            base.Start();
            if (this.Animator) {
                this.Animator.runtimeAnimatorController = this.Data.Animations;
            }
            
            this.AttributeSet.Initialise(this.Data.Attributes);
            this.LootContainer.Use(this.Data.LootTable);
        }

        public override void HandleDeath() {
            base.HandleDeath(); 
            broadcastEnemyDiedData.TriggerEvent(this, Data);
            Enemy.OnDeath?.Invoke(new EnemyDeathEvent(this.Data, this.LastAttacker));
        }
    }
}
