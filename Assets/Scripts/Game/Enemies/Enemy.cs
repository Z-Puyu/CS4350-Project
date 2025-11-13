using System.Collections.Generic;
using Events;
using Game.CharacterControls;
using ModularItemsAndInventory.Runtime.Items;
using ModularItemsAndInventory.Runtime.LootContainers;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;
using WeaponsSystem.Runtime.DamageHandling;
using WeaponsSystem.Runtime.Weapons;

namespace Game.Enemies {
    public class Enemy : GameCharacterController {
        public new static event UnityAction<EnemyDeathEvent> OnDeath;

        [field: SerializeField] protected EnemyData Data { get; set; }
        [field: SerializeField, Required] private Animator Animator { get; set; }
        [field: SerializeField, Required] private CrossObjectEventWithDataSO broadcastEnemyDiedData;
        [field: SerializeField, Required] private LootContainer LootContainer { get; set; }
        [field: SerializeField] private PickUp2D PickUpPrefab { get; set; }
        private GameObject LastAttacker { get; set; }

        public string getEnemyId() {
            return Data.Id;
        }

        public EnemyData GetEnemyData() {
            return Data;
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

            this.LastAttacker = GameObject.FindWithTag("Player");
            this.AttributeSet.Initialise(this.Data.Attributes);
            this.LootContainer.Use(this.Data.LootTable);
        }

        public override void HandleDeath() {
            broadcastEnemyDiedData.TriggerEvent(this, Data);
            base.HandleDeath();
            this.LootContainer.Open();
            foreach (KeyValuePair<ItemKey, int> drop in this.LootContainer) {
                Vector3 position = this.transform.position + new Vector3(
                    Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0
                );
                Object.Instantiate(this.PickUpPrefab, position, Quaternion.identity).With(drop.Value, drop.Key);
            }

            Enemy.OnDeath?.Invoke(new EnemyDeathEvent(this.Data, this.LastAttacker));
        }
    }
}
