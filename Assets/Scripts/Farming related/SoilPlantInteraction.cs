using UnityEngine;
using System.Collections;

public class SoilPlantInteraction : MonoBehaviour
{
    public enum PlantStage
    {
        Planted,
        Seedling,
        Grown,
        Wilting,
        Wilted
    }

    private PlantStage currentStage = PlantStage.Planted;
    private bool hasPlant = false;
    private bool isWatered = false;
    private int waterCount = 0;

    private Animator animator;

    // Harvest event for external scripts
    public delegate void OnHarvest();
    public event OnHarvest HarvestEvent;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // LEFT CLICK: Plant / Harvest
        if (Input.GetMouseButtonDown(0) && IsMouseOver())
        {
            if (!hasPlant)
                PlantSeed();
            else if (currentStage >= PlantStage.Grown)
                HarvestPlant();
        }

        // RIGHT CLICK: Water
        if (Input.GetMouseButtonDown(1) && IsMouseOver())
        {
            WaterPlant();
        }
    }

    bool IsMouseOver()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D col = GetComponent<Collider2D>();
        return col != null && col.OverlapPoint(mousePos);
    }

    // -----------------------
    void PlantSeed()
    {
        hasPlant = true;
        currentStage = PlantStage.Planted;
        isWatered = false;
        waterCount = 0;

        PlayPlantAnimation("dry");
    }

    void WaterPlant()
    {
        if (hasPlant && currentStage != PlantStage.Wilted)
        {
            if (!isWatered)
            {
                waterCount++;
                StartCoroutine(PlayPlantWateringThenWet());

                // Age after 2 waterings
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

    // -----------------------
    IEnumerator PlayPlantWateringThenWet()
    {
        isWatered = true;

        // Play watering animation once
        PlayPlantAnimation("watering");

        // Wait for actual watering animation duration (set according to your clip length)
        yield return new WaitForSeconds(4f);

        // Switch to looping wet animation
        PlayPlantAnimation("wet");

        // Stay wet for 10 seconds
        yield return new WaitForSeconds(10f);

        // Dry out after loop
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

    // -----------------------
    void HarvestPlant()
    {
        HarvestEvent?.Invoke();

        hasPlant = false;
        currentStage = PlantStage.Planted;
        isWatered = false;

        animator.SetTrigger("Harvest");
        animator.Play("dry_dirt", 0); // soil always stays dry
    }

    void AgePlant()
    {
        if (currentStage == PlantStage.Planted)
            currentStage = PlantStage.Seedling;
        else if (currentStage == PlantStage.Seedling)
            currentStage = PlantStage.Grown;
        else if (currentStage == PlantStage.Grown)
            currentStage = PlantStage.Wilting;
        else if (currentStage == PlantStage.Wilting)
            currentStage = PlantStage.Wilted;

        PlayPlantAnimation(isWatered ? "wet" : "dry");
    }

    void PlayPlantAnimation(string condition)
    {
        string animName = $"seed_1_{currentStage.ToString().ToLower()}_{condition}";
        animator.Play(animName, 0);
    }
}
