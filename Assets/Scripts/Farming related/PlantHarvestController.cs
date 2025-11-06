using System.Collections;
using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items;
using Player_related.Player_exp;
using UnityEngine;
using Game.Player;
using Events;

namespace Farming_related
{
    public class PlantHarvestController : MonoBehaviour
    {
        // external refs set by the SoilPlantInteraction owner
        private PlantGrowthController growthController;
        private PlantBuffProvider plantBuffProvider;
        private Animator animator;
        private PlayerController playerController;
        private PickUp2D pickUpPrefab;
        private CrossObjectEventWithDataSO broadcastFarmingExpObject;
        private SoilPlantInteraction owner;

        public void SetReferences(SoilPlantInteraction owner,
                                  PlantGrowthController gc,
                                  PlantBuffProvider pb,
                                  Animator anim,
                                  PlayerController pc,
                                  PickUp2D pickUp,
                                  CrossObjectEventWithDataSO broadcast)
        {
            this.owner = owner;
            this.growthController = gc;
            this.plantBuffProvider = pb;
            this.animator = anim;
            this.playerController = pc;
            this.pickUpPrefab = pickUp;
            this.broadcastFarmingExpObject = broadcast;
        }

        /// <summary>
        /// Perform the harvest flow: drop items, grant exp, remove buffs and reset growth.
        /// Returns true if an auto-replant occurred.
        /// </summary>
        public bool Harvest(string plantedSeedId, SoilPlantInteraction.PlantStage currentStage, FarmingExpObject farmingExpObject)
        {
            if (growthController == null || !growthController.HasPlant || currentStage < SoilPlantInteraction.PlantStage.Grown)
                return false;

            int cropCount = 1;
            int seedCount = 0;
            string dropItemId = plantedSeedId;

            switch (currentStage)
            {
                case SoilPlantInteraction.PlantStage.Grown:
                    dropItemId = plantedSeedId.Replace("seed", "crop");
                    DropItem(dropItemId, cropCount);
                    seedCount = Random.Range(1, 3);
                    broadcastFarmingExpObject?.TriggerEvent(owner, farmingExpObject);
                    break;
                case SoilPlantInteraction.PlantStage.Wilting:
                    dropItemId = plantedSeedId.Replace("seed", "wilting");
                    DropItem(dropItemId, cropCount);
                    seedCount = Random.Range(0, 2);
                    broadcastFarmingExpObject?.TriggerEvent(owner, farmingExpObject);
                    break;
                case SoilPlantInteraction.PlantStage.Wilted:
                    seedCount = Random.Range(0, 2);
                    cropCount = 0;
                    break;
            }

            if (seedCount > 0) DropItem(plantedSeedId, seedCount);

            // Reset plant state
            plantBuffProvider?.RemoveBuff();
            growthController?.ResetPlant();
            // stop any coroutines that might be running on this GameObject
            StopAllCoroutines();

            if (animator != null)
            {
                animator.SetTrigger("Harvest");
                animator.Play("dry_dirt", 0);
            }

            // Try auto replant if player has the feature
            bool didAutoReplant = false;
            if (playerController != null && playerController.HasAutoReplant)
            {
                didAutoReplant = TryAutoReplant(plantedSeedId);
            }

            return didAutoReplant;
        }

        private bool TryAutoReplant(string seedId)
        {
            if (string.IsNullOrEmpty(seedId) || playerController == null || owner == null)
                return false;

            Inventory inventory = null;
            Transform root = playerController.transform.root;
            if (root != null)
            {
                inventory = root.GetComponentInChildren<Inventory>(true);
            }

            if (inventory == null)
            {
                Debug.LogWarning("⚠️ Player inventory not found under Player root — auto replant cancelled.");
                return false;
            }

            if (ItemDatabase.TryGet(seedId, out ItemData data))
            {
                ItemKey seedKey = ItemKey.From(data);
                int count = inventory.Count(seedKey);
                if (count > 0)
                {
                    inventory.Remove(seedKey);
                    Debug.Log($"🌱 Auto replanted {seedId} from inventory (removed key '{seedKey}').");
                    owner.PlantSeed(seedId);
                    return true;
                }
                else
                {
                    Debug.LogWarning($"⚠️ Auto replant failed — no item for key '{seedKey}' in this inventory.");
                    Debug.Log("Inventory contents:");
                    foreach (var kv in inventory)
                    {
                        ItemKey key = kv.Key;
                        int qty = kv.Value;
                        Debug.Log($" - id='{key.Id}', qty={qty}");
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Seed item {seedId} not found in database for auto replant.");
            }

            return false;
        }

        private void DropItem(string itemId, int count)
        {
            if (count <= 0 || pickUpPrefab == null) return;
            if (!ItemDatabase.TryGet(itemId, out ItemData itemData)) return;

            Item item = Item.From(itemData);

            for (int i = 0; i < count; i++)
            {
                Vector3 position = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                Object.Instantiate(pickUpPrefab, position, Quaternion.identity).With(1, item.Key);
            }
        }
    }
}
