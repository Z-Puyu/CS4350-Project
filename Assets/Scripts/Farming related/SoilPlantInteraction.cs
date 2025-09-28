using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Inventory_related.Inventory_UI_Manager;
using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items;
using Common;

public class SoilPlantInteraction : MonoBehaviour
{
    public enum PlantStage { Planted, Seedling, Grown, Wilting, Wilted }

    [Header("Plant Info")]
    [SerializeField] private string plantedSeedId; // which seed type (from inventory)
    [SerializeField] private PickUp2D pickUpPrefab;
    
    [Header("Growth Settings")]
    [SerializeField] private Image growthBar;
    [SerializeField] private float baseGrowthSpeed = 0.01f;   // per second when dry
    [SerializeField] private float wetGrowthMultiplier = 3f;  // speed multiplier when wet

    private PlantStage currentStage = PlantStage.Planted;
    private bool hasPlant = false;
    private bool isWatered = false;
    private int waterCount = 0;
    private bool playerIsOver = false;
    private float growthProgress = 0f;

    private Animator animator;

    public delegate void OnHarvest();
    public event OnHarvest HarvestEvent;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleInput();
        UpdateGrowth();
    }

    #region Input Handling
    void HandleInput()
    {
        if (!playerIsOver) return;

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (hasPlant && (currentStage >= PlantStage.Grown))
            {
                HarvestPlant();
            }
            else if (!hasPlant)
            {
                OpenInventoryForSeed();
            }
        }

        if (Input.GetKeyDown(KeyCode.V) && hasPlant)
        {
            WaterPlant();
        }
    }
    #endregion

    #region Growth System
    void UpdateGrowth()
    {
        if (!hasPlant) 
        {
            growthBar.fillAmount = 0f;
            return;
        }

        // Increase growth progress
        float growthThisFrame = baseGrowthSpeed * Time.deltaTime;
        if (isWatered) growthThisFrame *= wetGrowthMultiplier;

        growthProgress += growthThisFrame;
        growthProgress = Mathf.Clamp01(growthProgress);

        // Update progress bar
        growthBar.fillAmount = growthProgress;
        UpdateGrowthBarColor();

        // Automatically update plant stage
        UpdatePlantStageByProgress();
    }

    void UpdatePlantStageByProgress()
    {
        if (growthProgress < 0.25f) currentStage = PlantStage.Planted;
        else if (growthProgress < 0.50f) currentStage = PlantStage.Seedling;
        else if (growthProgress < 0.75f) currentStage = PlantStage.Grown;
        else if (growthProgress < 1f) currentStage = PlantStage.Wilting;
        else currentStage = PlantStage.Wilted;

        // Update animation
        PlayPlantAnimation(isWatered ? "wet" : "dry");
    }

    void UpdateGrowthBarColor()
    {
        if (growthProgress < 0.5f) growthBar.color = Color.green;
        else if (growthProgress < 0.75f) growthBar.color = new Color(0.6f, 0f, 0.6f); // purple
        else growthBar.color = Color.red;
    }
    #endregion

    #region Trigger Handling
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerIsOver = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerIsOver = false;
    }
    #endregion

    #region Planting & Watering
    void OpenInventoryForSeed()
    {
        InventoryUIManager.Instance.OpenForSeedSelection(this);
    }

    public void PlantSeed(string seedId)
    {
        plantedSeedId = seedId;
        hasPlant = true;
        currentStage = PlantStage.Planted;
        growthProgress = 0f;
        isWatered = false;
        waterCount = 0;

        PlayPlantAnimation("dry");
    }

    void WaterPlant()
    {
        if (!hasPlant || currentStage == PlantStage.Wilted) return;

        if (!isWatered)
        {
            isWatered = true;
            waterCount++;
            StartCoroutine(PlayPlantWateringThenWet());
        }
    }

    IEnumerator PlayPlantWateringThenWet()
    {
        PlayPlantAnimation("watering");
        yield return new WaitForSeconds(4f);

        PlayPlantAnimation("wet");
        yield return new WaitForSeconds(10f);

        isWatered = false;
        if (hasPlant && currentStage != PlantStage.Wilted)
            PlayPlantAnimation("dry");
    }
    #endregion

    #region Harvest
    void HarvestPlant()
    {
        if (!hasPlant || currentStage < PlantStage.Grown) return;

        HarvestEvent?.Invoke();

        // Determine drop counts
        int cropCount = 1;
        int seedCount = 0;
        string dropItemId = plantedSeedId;

        switch (currentStage)
        {
            case PlantStage.Grown:
                dropItemId = plantedSeedId.Replace("seed", "crop");
                DropItem(dropItemId, cropCount);
                seedCount = Random.Range(1, 3);
                break;
            case PlantStage.Wilting:
                dropItemId = plantedSeedId.Replace("seed", "wilting");
                DropItem(dropItemId, cropCount);
                seedCount = Random.Range(0, 2);
                break;
            case PlantStage.Wilted:
                seedCount = Random.Range(0, 2);
                cropCount = 0;
                break;
        }

        if (seedCount > 0) DropItem(plantedSeedId, seedCount);

        // Reset plant
        plantedSeedId = null;
        hasPlant = false;
        currentStage = PlantStage.Planted;
        isWatered = false;
        growthProgress = 0f;

        if (animator != null)
        {
            animator.SetTrigger("Harvest");
            animator.Play("dry_dirt", 0);
        }
    }

    private void DropItem(string itemId, int count)
    {
        if (count <= 0 || pickUpPrefab == null) return;

        if (!ItemDatabase.TryGet(itemId, out ItemData itemData))
            return;

        Item item = Item.From(itemData);

        for (int i = 0; i < count; i++)
        {
            Vector3 position = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            Object.Instantiate(pickUpPrefab, position, Quaternion.identity).With(1, item.Key);
        }
    }
    #endregion

    #region Animation
    void PlayPlantAnimation(string condition)
    {
        if (string.IsNullOrEmpty(plantedSeedId) || animator == null) return;

        string animName = $"{plantedSeedId}_{currentStage.ToString().ToLower()}_{condition}";
        animator.Play(animName, 0);
    }
    #endregion
}
