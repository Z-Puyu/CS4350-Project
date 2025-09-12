using System;
using System.Collections.Generic;
using InteractionSystem.Runtime;
using ModularItemsAndInventory.Runtime.Items;
using ModularItemsAndInventory.Runtime.LootContainers;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Game.MapObjects {
    [DisallowMultipleComponent, RequireComponent(typeof(LootContainer), typeof(Interactable))]
    public class TreasureChest : MonoBehaviour {
        private LootContainer LootContainer { get; set; }
        [field: SerializeField] private PickUp2D PickUpPrefab { get; set; }

        private void Awake() {
            this.LootContainer = this.GetComponent<LootContainer>();
        }
        
        private void CreatePickUp(ItemKey item, int count = 1) {
            Vector3 position = this.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            Object.Instantiate(this.PickUpPrefab, position, Quaternion.identity).With(count, item);
        }

        public void Dismantle(Interactor interactor) {
            this.LootContainer.Open();
            foreach (KeyValuePair<ItemKey, int> drop in this.LootContainer) {
                for (int i = 0; i < drop.Value; i += 1) {
                    this.CreatePickUp(drop.Key);
                }
            }
            
            Object.Destroy(this.gameObject);
        }
    }
}
