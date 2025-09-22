using UnityEngine;
using System.Collections;
using Inventory_related.Inventory_UI_Manager;
using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items;
using Common;

public class SoilPlantInteraction : MonoBehaviour
{
    public enum PlantStage { Planted, Seedling, Grown, Wilting, Wilted }

    [SerializeField] private string plantedSeedId; // which seed type (from inventory)
    [SerializeField] private PickUp2D pickUpPrefab;
    private PlantStage currentStage = PlantStage.Planted;
    private bool hasPlant = false;
    private bool isWatered = false;
    private int waterCount = 0;
    private bool playerIsOver = false;

    private Animator animator;

    public delegate void OnHarvest();
    public event OnHarvest HarvestEvent;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            OnScreenDebugger.Log("C key pressed");

            if (playerIsOver)
            {
                if (hasPlant && (currentStage == PlantStage.Grown || currentStage == PlantStage.Wilting))
                {
                    OnScreenDebugger.Log("Plant is ready → harvesting");
                    HarvestPlant();
                }
                else if (!hasPlant)
                {
                    OnScreenDebugger.Log("Player is over soil → opening inventory");
                    OpenInventoryForSeed();
                }
                else
                {
                    OnScreenDebugger.Log("Seed planted, do nothing.");
                }
            }
            else
            {
                OnScreenDebugger.Log("Player is NOT over soil → cannot interact");
            }
        }

        if (Input.GetKeyDown(KeyCode.V) && playerIsOver)
        {
            OnScreenDebugger.Log("V key pressed → watering plant");
            WaterPlant();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Make sure your player GameObject has the tag "Player"
        {
            playerIsOver = true;
            OnScreenDebugger.Log("Player entered soil tile");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsOver = false;
            OnScreenDebugger.Log("Player left soil tile");
        }
    }

    // -----------------------
    void OpenInventoryForSeed()
    {
        Debug.Log("Opening inventory to choose a seed...");
        InventoryUIManager.Instance.OpenForSeedSelection(this);
    }

    // Called by Inventory when a seed is chosen
    public void PlantSeed(string seedId)
    {
        plantedSeedId = seedId;
        hasPlant = true;
        currentStage = PlantStage.Planted;
        isWatered = false;
        waterCount = 0;

        PlayPlantAnimation("dry");
        Debug.Log($"Planted {seedId} in soil!");
    }

    // -----------------------
    void WaterPlant()
    {
        if (hasPlant && currentStage != PlantStage.Wilted)
        {
            if (!isWatered)
            {
                waterCount++;
                StartCoroutine(PlayPlantWateringThenWet());

                if (waterCount >= 2)
                {
                    waterCount = 0;
                    StartCoroutine(AgePlantAfterDelay(5f));
                }
            }
            else
            {
                Debug.Log("Already watered. Wait until it dries out.");
            }
        }
    }

    IEnumerator PlayPlantWateringThenWet()
    {
        isWatered = true;
        PlayPlantAnimation("watering");
        yield return new WaitForSeconds(4f);

        PlayPlantAnimation("wet");
        yield return new WaitForSeconds(10f);

        isWatered = false;
        if (hasPlant && currentStage != PlantStage.Wilted)
            PlayPlantAnimation("dry");
    }

    IEnumerator AgePlantAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (hasPlant && isWatered)
            AgePlant();
    }

    void HarvestPlant()
    {
        if (!hasPlant || currentStage < PlantStage.Grown)
        {
            Debug.Log("Plant not ready to harvest.");
            return;
        }

        HarvestEvent?.Invoke();

        Debug.Log($"Harvesting {plantedSeedId} at stage {currentStage}...");

        // Determine drop counts based on stage
        int cropCount = 1; // always drop 1 crop
        int seedCount = 0;
        string dropItemId;

        switch (currentStage)
        {
            case PlantStage.Grown:
                dropItemId = plantedSeedId.Replace("seed", "crop");
                DropItem(dropItemId, cropCount);
                seedCount = UnityEngine.Random.Range(1, 3); // 1-2 seeds
                break;
            case PlantStage.Wilting:
                dropItemId = plantedSeedId.Replace("seed", "wilting");
                DropItem(dropItemId, cropCount);
                seedCount = UnityEngine.Random.Range(0, 2); // 0-1 seeds
                break;
            case PlantStage.Wilted:
                seedCount = UnityEngine.Random.Range(0, 2); // 0-1 seeds
                cropCount = 0; // maybe nothing if fully wilted
                break;
        }

        // Drop seeds
        if (seedCount > 0)
            DropItem(plantedSeedId, seedCount);

        // Reset plant
        plantedSeedId = null;
        hasPlant = false;
        currentStage = PlantStage.Planted;
        isWatered = false;

        // Play harvest animation
        if (animator != null)
        {
            animator.SetTrigger("Harvest");
            animator.Play("dry_dirt", 0);
        }
    }

    private void DropItem(string itemId, int count)
    {
        Debug.Log($"Dropping {count} {itemId}...");

        if (count <= 0 || pickUpPrefab == null) return;

        if (!ItemDatabase.TryGet(itemId, out ItemData itemData))
        {
            Debug.LogWarning($"Item {itemId} not found in database.");
            return;
        }

        Item item = Item.From(itemData);

        for (int i = 0; i < count; i++)
        {
            Vector3 position = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            Object.Instantiate(pickUpPrefab, position, Quaternion.identity).With(1, item.Key);
        }
    }

    void AgePlant()
    {
        if (currentStage == PlantStage.Planted) currentStage = PlantStage.Seedling;
        else if (currentStage == PlantStage.Seedling) currentStage = PlantStage.Grown;
        else if (currentStage == PlantStage.Grown) currentStage = PlantStage.Wilting;
        else if (currentStage == PlantStage.Wilting) currentStage = PlantStage.Wilted;

        PlayPlantAnimation(isWatered ? "wet" : "dry");
    }

    void PlayPlantAnimation(string condition)
    {
        string animName = $"{plantedSeedId}_{currentStage.ToString().ToLower()}_{condition}";
        animator.Play(animName, 0);
    }
}
