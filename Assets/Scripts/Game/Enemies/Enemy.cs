using Events;
using Game.CharacterControls;
using ModularItemsAndInventory.Runtime.LootContainers;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;
using WeaponsSystem.Runtime.DamageHandling;
using WeaponsSystem.Runtime.Weapons;

namespace Game.Enemies {
    public class Enemy : GameCharacterController {
        public static event UnityAction<EnemyDeathEvent> OnDeath; 
        
        [field: SerializeField] private EnemyData Data { get; set; }
        [field: SerializeField, Required] private Animator Animator { get; set; }
        [field: SerializeField, Required] private CrossObjectEventWithDataSO broadcastEnemyDiedData;
        [field: SerializeField, Required] private LootContainer LootContainer { get; set; }
        private GameObject LastAttacker { get; set; }

        public string getEnemyId() {
            return Data.Id;
        }

        protected override void ConfigureAttributeSet() {
            this.AttributeSet.Initialise(this.Data.Attributes);
        }

        public void RememberAttacker(Damage damage) {
            this.LastAttacker = damage.Instigator;
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
            broadcastEnemyDiedData.TriggerEvent(this, Data);
            base.HandleDeath(); 
            Enemy.OnDeath?.Invoke(new EnemyDeathEvent(this.Data, this.LastAttacker));
        }
    }
}
