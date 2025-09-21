using System;
using System.Collections.Generic;
using Common;
using Events;
using Game.Enemies;
using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using InteractionSystem.Runtime;
using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items;
using SaintsField;
using UnityEngine;
using WeaponsSystem;
using WeaponsSystem.DamageHandling;

namespace Game.Player {
    [DisallowMultipleComponent]
    public sealed class PlayerController : MonoBehaviour, ICollector {
        public CrossObjectEventWithDataSO broadcastItemCollected;
        [field: SerializeField] private PlayerData InitialData { get; set; }
        [field: SerializeField, Required] private AttributeSet AttributeSet { get; set; }
        [field: SerializeField, Required] private Inventory Inventory { get; set; }
        [field: SerializeField, Required] private AbilitySystem AbilitySystem { get; set; }

        private void Awake() {
            foreach (HitBox2D hitbox in this.GetComponentsInChildren<HitBox2D>()) {
                hitbox.OnHit += this.HandleDamage;
            }
        }

        private void Start() {
            if (!this.InitialData) {
                return;
            }
            
            this.ConfigureAttributeSet();
            this.ConfigureInventory();
            Enemy.OnDeath += this.HandleEnemyDeath;
            this.GetComponentInChildren<Interactor>().OnInteract += obj => this.Say("Interacted with " + obj.name);
            this.GetComponentInChildren<Combatant>().Equip(this.GetComponentInChildren<IDamageDealer>());
        }
        
        private void HandleDamage(Damage damage) {
            GameObject source = damage.Instigator;
            AbilitySystem instigator = source.GetComponentInChildren<AbilitySystem>();
            if (!instigator) {
                Debug.LogError($"{source.name} must have an Ability System to attack the player!", source);
            } else {
                this.Say($"{source.name} damaged the player!");
                GameplayEffectExecutionArgs args = instigator.CreateEffectExecutionArgs()
                                                             .WithUserData(damage.Data)
                                                             .Build();
                instigator.Use("basic:attack", this.AbilitySystem, args);
            }
        }

        private void ConfigureInventory() {
            foreach (KeyValuePair<ItemData, int> data in this.InitialData.Items) {
                this.Inventory.Add(data.Value, ItemKey.From(data.Key));
            }
        }
        
        private void ConfigureAttributeSet() {
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

        public void Say(string message) {
            OnScreenDebugger.Log(message);
        }
    }
}
