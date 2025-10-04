using System.Collections;
using Inventory_related.Inventory_UI_Manager;
using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Farming_related {
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
            this.animator = this.GetComponent<Animator>();
        }

        void Update()
        {
            this.HandleInput();
            this.UpdateGrowth();
        }

        #region Input Handling
        void HandleInput()
        {
            if (!this.playerIsOver) return;

            if (Input.GetKeyDown(KeyCode.C))
            {
                if (this.hasPlant && (this.currentStage >= PlantStage.Grown))
                {
                    this.HarvestPlant();
                }
                else if (!this.hasPlant)
                {
                    this.OpenInventoryForSeed();
                }
            }

            if (Input.GetKeyDown(KeyCode.F) && this.hasPlant)
            {
                this.WaterPlant();
            }
        }
        #endregion

        #region Growth System
        void UpdateGrowth()
        {
            if (!this.hasPlant) 
            {
                this.growthBar.fillAmount = 0f;
                return;
            }

            // Increase growth progress
            float growthThisFrame = this.baseGrowthSpeed * Time.deltaTime;
            if (this.isWatered) growthThisFrame *= this.wetGrowthMultiplier;

            this.growthProgress += growthThisFrame;
            this.growthProgress = Mathf.Clamp01(this.growthProgress);

            // Update progress bar
            this.growthBar.fillAmount = this.growthProgress;
            this.UpdateGrowthBarColor();

            // Automatically update plant stage
            this.UpdatePlantStageByProgress();
        }

        void UpdatePlantStageByProgress()
        {
            if (this.growthProgress < 0.25f) this.currentStage = PlantStage.Planted;
            else if (this.growthProgress < 0.50f) this.currentStage = PlantStage.Seedling;
            else if (this.growthProgress < 0.75f) this.currentStage = PlantStage.Grown;
            else if (this.growthProgress < 1f) this.currentStage = PlantStage.Wilting;
            else this.currentStage = PlantStage.Wilted;

            // Update animation
            this.PlayPlantAnimation(this.isWatered ? "wet" : "dry");
        }

        void UpdateGrowthBarColor()
        {
            if (this.growthProgress < 0.5f) this.growthBar.color = Color.green;
            else if (this.growthProgress < 0.75f) this.growthBar.color = new Color(0.6f, 0f, 0.6f); // purple
            else this.growthBar.color = Color.red;
        }
        #endregion

        #region Trigger Handling
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.transform.root.CompareTag("Player"))
            {
                this.playerIsOver = true;
            }

            Debug.Log($"Player is over soil: {this.playerIsOver}");
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.transform.root.CompareTag("Player"))
            {
                this.playerIsOver = false;
            }

            Debug.Log($"Player is over soil: {this.playerIsOver}");
        }
        #endregion

        #region Planting & Watering
        void OpenInventoryForSeed()
        {
            InventoryUIManager.Instance.OpenForSeedSelection(this);
        }

        public void PlantSeed(string seedId)
        {
            this.plantedSeedId = seedId;
            this.hasPlant = true;
            this.currentStage = PlantStage.Planted;
            this.growthProgress = 0f;
            this.isWatered = false;
            this.waterCount = 0;

            this.PlayPlantAnimation("dry");
        }

        void WaterPlant()
        {
            if (!this.hasPlant || this.currentStage == PlantStage.Wilted) return;

            if (!this.isWatered)
            {
                this.isWatered = true;
                this.waterCount++;
                this.StartCoroutine(this.PlayPlantWateringThenWet());
            }
        }

        IEnumerator PlayPlantWateringThenWet()
        {
            this.PlayPlantAnimation("watering");
            yield return new WaitForSeconds(4f);

            this.PlayPlantAnimation("wet");
            yield return new WaitForSeconds(10f);

            this.isWatered = false;
            if (this.hasPlant && this.currentStage != PlantStage.Wilted)
                this.PlayPlantAnimation("dry");
        }
        #endregion

        #region Harvest
        void HarvestPlant()
        {
            if (!this.hasPlant || this.currentStage < PlantStage.Grown) return;

            this.HarvestEvent?.Invoke();

            // Determine drop counts
            int cropCount = 1;
            int seedCount = 0;
            string dropItemId = this.plantedSeedId;

            switch (this.currentStage)
            {
                case PlantStage.Grown:
                    dropItemId = this.plantedSeedId.Replace("seed", "crop");
                    this.DropItem(dropItemId, cropCount);
                    seedCount = Random.Range(1, 3);
                    break;
                case PlantStage.Wilting:
                    dropItemId = this.plantedSeedId.Replace("seed", "wilting");
                    this.DropItem(dropItemId, cropCount);
                    seedCount = Random.Range(0, 2);
                    break;
                case PlantStage.Wilted:
                    seedCount = Random.Range(0, 2);
                    cropCount = 0;
                    break;
            }

            if (seedCount > 0) this.DropItem(this.plantedSeedId, seedCount);

            // Reset plant
            this.plantedSeedId = null;
            this.hasPlant = false;
            this.currentStage = PlantStage.Planted;
            this.isWatered = false;
            this.growthProgress = 0f;

            if (this.animator != null)
            {
                this.animator.SetTrigger("Harvest");
                this.animator.Play("dry_dirt", 0);
            }
        }

        private void DropItem(string itemId, int count)
        {
            if (count <= 0 || this.pickUpPrefab == null) return;

            if (!ItemDatabase.TryGet(itemId, out ItemData itemData))
                return;

            Item item = Item.From(itemData);

            for (int i = 0; i < count; i++)
            {
                Vector3 position = this.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                Object.Instantiate(this.pickUpPrefab, position, Quaternion.identity).With(1, item.Key);
            }
        }
        #endregion

        #region Animation
        void PlayPlantAnimation(string condition)
        {
            if (string.IsNullOrEmpty(this.plantedSeedId) || this.animator == null) return;

            string animName = $"{this.plantedSeedId}_{this.currentStage.ToString().ToLower()}_{condition}";
            this.animator.Play(animName, 0);
        }
        #endregion
    }
}
