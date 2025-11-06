using System;
using System.Collections;
using UnityEngine;

namespace Farming_related
{
    /// <summary>
    /// Owns growth progress, stage transitions and watering behaviour for a plant.
    /// It runs its own Update loop and exposes simple events for the owner to react
    /// (animations, UI updates, buffs).
    /// </summary>
    public class PlantGrowthController : MonoBehaviour
    {
        public SoilPlantInteraction.PlantStage CurrentStage { get; private set; } = SoilPlantInteraction.PlantStage.Planted;
        public float GrowthProgress { get; private set; } = 0f;
        public bool HasPlant { get; private set; } = false;
        public bool IsWatered { get; private set; } = false;

        // configuration
        private float baseGrowthSpeed = 1f;
        private float wetGrowthMultiplier = 5f;
        private int requiredWaterings = 0;
        private int currentWaterings = 0;

        // thresholds (normalized 0..1)
        private float plantedThreshold = 0.25f;
        private float seedlingThreshold = 0.50f;
        private float grownThreshold = 0.75f;
        private float wiltingThreshold = 1.0f;

        // watering animation timing (kept same as previous behaviour)
        private float wateringAnimDuration = 4f;
        private float wetDuration = 10f;

        public event Action<SoilPlantInteraction.PlantStage, SoilPlantInteraction.PlantStage> OnStageChanged;
        public event Action<float> OnProgressChanged;
        public event Action<bool> OnWaterStateChanged;

        // initialize with optional thresholds and multipliers
        public void Initialize(float durationSeconds, int wateringRequirement, float wetMultiplier, float planted, float seedling, float grown, float wilting)
        {
            baseGrowthSpeed = Mathf.Max(0.00001f, 1f / Mathf.Max(1f, durationSeconds));
            requiredWaterings = Mathf.Max(0, wateringRequirement);
            wetGrowthMultiplier = Mathf.Max(1f, wetMultiplier);

            plantedThreshold = Mathf.Clamp01(planted);
            seedlingThreshold = Mathf.Clamp01(seedling);
            grownThreshold = Mathf.Clamp01(grown);
            wiltingThreshold = Mathf.Clamp01(wilting);

            // ensure monotonicity
            if (seedlingThreshold < plantedThreshold) seedlingThreshold = plantedThreshold;
            if (grownThreshold < seedlingThreshold) grownThreshold = seedlingThreshold;
            if (wiltingThreshold < grownThreshold) wiltingThreshold = grownThreshold;
        }

        public void Plant()
        {
            HasPlant = true;
            GrowthProgress = 0f;
            CurrentStage = SoilPlantInteraction.PlantStage.Planted;
            currentWaterings = 0;
            IsWatered = false;
            OnProgressChanged?.Invoke(GrowthProgress);
            OnStageChanged?.Invoke(SoilPlantInteraction.PlantStage.Wilted, CurrentStage);
        }

        public void ResetPlant()
        {
            HasPlant = false;
            GrowthProgress = 0f;
            CurrentStage = SoilPlantInteraction.PlantStage.Planted;
            currentWaterings = 0;
            IsWatered = false;
            OnProgressChanged?.Invoke(GrowthProgress);
            OnStageChanged?.Invoke(CurrentStage, CurrentStage);
        }

        void Update()
        {
            if (!HasPlant) return;

            // Determine if watering should block growth (only for Planted and Seedling)
            bool stageRequiresWatering = (CurrentStage == SoilPlantInteraction.PlantStage.Planted || CurrentStage == SoilPlantInteraction.PlantStage.Seedling)
                                        && requiredWaterings > 0;

            if (stageRequiresWatering)
            {
                var bounds = GetStageBounds(CurrentStage);

                float wateringRatio = Mathf.Clamp01((float)currentWaterings / requiredWaterings);
                float stageCap = Mathf.Lerp(bounds.start, bounds.end, wateringRatio);

                if (GrowthProgress >= stageCap)
                {
                    if (IsWatered)
                    {
                        IsWatered = false;
                        StopAllCoroutines(); // stop watering coroutine if running
                        OnWaterStateChanged?.Invoke(IsWatered);
                    }

                    OnProgressChanged?.Invoke(GrowthProgress);
                    return; // stop growth for this frame
                }
            }

            // Normal growth
            float growthThisFrame = baseGrowthSpeed * Time.deltaTime;
            if (IsWatered) growthThisFrame *= wetGrowthMultiplier;

            GrowthProgress += growthThisFrame;
            GrowthProgress = Mathf.Clamp01(GrowthProgress);

            OnProgressChanged?.Invoke(GrowthProgress);

            // Update stage after applying watering check
            var previousStage = CurrentStage;
            UpdatePlantStageByProgress();

            if (CurrentStage != previousStage)
            {
                // reset waterings when advancing a stage that still requires watering
                if (CurrentStage <= SoilPlantInteraction.PlantStage.Seedling)
                {
                    currentWaterings = 0;
                }

                OnStageChanged?.Invoke(previousStage, CurrentStage);
            }
        }

        public void WaterPlant()
        {
            if (!HasPlant) return;
            if (IsWatered) return;

            IsWatered = true;
            currentWaterings++;
            OnWaterStateChanged?.Invoke(IsWatered);

            StartCoroutine(PlayWateringThenWet());
        }

        IEnumerator PlayWateringThenWet()
        {
            // matches previous behaviour timing
            yield return new WaitForSeconds(wateringAnimDuration);

            // stay wet for wetDuration
            yield return new WaitForSeconds(wetDuration);

            IsWatered = false;
            OnWaterStateChanged?.Invoke(IsWatered);
        }

        private void UpdatePlantStageByProgress()
        {
            if (GrowthProgress < plantedThreshold) CurrentStage = SoilPlantInteraction.PlantStage.Planted;
            else if (GrowthProgress < seedlingThreshold) CurrentStage = SoilPlantInteraction.PlantStage.Seedling;
            else if (GrowthProgress < grownThreshold) CurrentStage = SoilPlantInteraction.PlantStage.Grown;
            else if (GrowthProgress < wiltingThreshold) CurrentStage = SoilPlantInteraction.PlantStage.Wilting;
            else CurrentStage = SoilPlantInteraction.PlantStage.Wilted;
        }

        private (float start, float end) GetStageBounds(SoilPlantInteraction.PlantStage stage)
        {
            switch (stage)
            {
                case SoilPlantInteraction.PlantStage.Planted:
                    return (0f, plantedThreshold);
                case SoilPlantInteraction.PlantStage.Seedling:
                    return (plantedThreshold, seedlingThreshold);
                case SoilPlantInteraction.PlantStage.Grown:
                    return (seedlingThreshold, grownThreshold);
                case SoilPlantInteraction.PlantStage.Wilting:
                    return (grownThreshold, wiltingThreshold);
                case SoilPlantInteraction.PlantStage.Wilted:
                    return (wiltingThreshold, 1f);
                default:
                    return (0f, 1f);
            }
        }
    }
}
