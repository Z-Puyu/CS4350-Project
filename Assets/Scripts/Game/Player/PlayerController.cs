using System.Collections.Generic;
using Common;
using Events;
using Game.Enemies;
using InteractionSystem.Runtime;
using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items;
using SaintsField;
using UnityEngine;
using WeaponsSystem;
using WeaponsSystem.DamageHandling;
using CharacterController = Game.CharacterControls.CharacterController;

namespace Game.Player {
    [DisallowMultipleComponent]
    public sealed class PlayerController : CharacterController, ICollector {
        public CrossObjectEventWithDataSO broadcastItemCollected;
        [field: SerializeField] private PlayerData InitialData { get; set; }
        [field: SerializeField, Required] private Inventory Inventory { get; set; }
        
        protected override void Start() {
            if (!this.InitialData) {
                return;
            }
            
            base.Start();
            this.ConfigureInventory();
            Enemy.OnDeath += this.HandleEnemyDeath;
            this.GetComponentInChildren<Interactor>().OnInteract += obj => this.Say("Interacted with " + obj.name);
            this.GetComponentInChildren<Combatant>().Equip(this.GetComponentInChildren<IDamageDealer>());
        }

        private void ConfigureInventory() {
            foreach (KeyValuePair<ItemData, int> data in this.InitialData.Items) {
                this.Inventory.Add(data.Value, ItemKey.From(data.Key));
            }
        }
        
        protected override void ConfigureAttributeSet() {
            this.AttributeSet.Initialise(this.InitialData.Attributes);
        }

        private void HandleEnemyDeath(EnemyDeathEvent @event) {
            if (@event.Killer != this.gameObject && @event.Killer.transform.IsChildOf(this.transform)) {
                return;
            }
        }

        public void Collect(int count, ItemKey item) {
            this.Inventory.Add(count, item);
            broadcastItemCollected.TriggerEvent(this, item);
            OnScreenDebugger.Log($"Collected {count} {item.Id}");
            OnScreenDebugger.Log("Current Inventory:");
            foreach (KeyValuePair<ItemKey, int> pair in this.Inventory) {
                OnScreenDebugger.Log($"{pair.Key.Id}: {pair.Value}");
            }
        }
    }
}
