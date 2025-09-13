using System;
using System.Collections.Generic;
using Common;
using Game.Enemies;
using Game.Objectives;
using GameplayAbilities.Runtime.Attributes;
using InteractionSystem.Runtime;
using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items;
using QuestAndObjective.Runtime;
using SaintsField;
using UnityEngine;

namespace Player {
    [DisallowMultipleComponent]
    public sealed class PlayerController : MonoBehaviour, ICollector {
        [field: SerializeField] private PlayerData InitialData { get; set; }
        [field: SerializeField, Required] private AttributeSet AttributeSet { get; set; }
        [field: SerializeField, Required] private Inventory Inventory { get; set; }
        [field: SerializeField, Required] private QuestLog QuestLog { get; set; }
        private GlobalQuestProgressProvider GlobalQuestProgress { get; set; } = new GlobalQuestProgressProvider();
        
        private void Start() {
            if (!this.InitialData) {
                return;
            }
            
            this.ConfigureAttributeSet();
            this.ConfigureInventory();
            this.ConfigureQuestLog();
            Enemy.OnDeath += this.HandleEnemyDeath;
            this.GetComponentInChildren<Interactor>().OnInteract += obj => this.Say("Interacted with " + obj.name);
        }

        private void ConfigureInventory() {
            foreach (KeyValuePair<ItemData, int> data in this.InitialData.Items) {
                this.Inventory.Add(data.Value, ItemKey.From(data.Key));
            }
        }
        
        private void ConfigureAttributeSet() {
            this.AttributeSet.Initialise(this.InitialData.Attributes);
        }

        private void ConfigureQuestLog() {
            IQuestProgressProvider questProgressProvider = this.GlobalQuestProgress
                                                               .And(new InventoryQuestProgressProvider(this.Inventory));
            this.QuestLog.WithQuestProgressProvider(questProgressProvider);
        }

        private void HandleEnemyDeath(EnemyDeathEvent @event) {
            if (@event.Killer != this.gameObject && @event.Killer.transform.IsChildOf(this.transform)) {
                return;
            }
            
            this.GlobalQuestProgress.UpdateVariable($"{@event.DeadEnemy.Id}:kill_count", 1);
        }

        public void Collect(int count, ItemKey item) {
            this.Inventory.Add(count, item);
            OnScreenDebugger.Log($"Collected {count} {item.Id}");
            OnScreenDebugger.Log("Current Inventory:");
            foreach (KeyValuePair<ItemKey, int> pair in this.Inventory) {
                OnScreenDebugger.Log($"{pair.Key.Id}: {pair.Value}");
            }
        }

        public void Say(string message) {
            OnScreenDebugger.Log(message);
        }
    }
}
