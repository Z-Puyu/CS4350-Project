using System.Collections;
using Events;
using Game.Player;
using Inventory_related.Inventory_UI_Manager;
using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items;
using Player_related.Player_exp;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Farming_related {
    public class SoilPlantInteraction : MonoBehaviour
    {
        public enum PlantStage { Planted, Seedling, Grown, Wilting, Wilted }

        [Header("UI")]
        [SerializeField] private GameObject promptUI; // assign your floating TMP prefab here
        [SerializeField] private TMP_Text promptText;
        [SerializeField] private float promptDistance = 2f; // distance at which popup appears

        [Header("Plant Info")]
        [SerializeField] private string plantedSeedId; // which seed type (from inventory)
        [SerializeField] private PickUp2D pickUpPrefab;
    
        [Header("Growth Settings")]
        [SerializeField] private Image growthBar;
        [SerializeField] private float baseGrowthSpeed = 0.01f;   // per second when dry
        [SerializeField] private float wetGrowthMultiplier = 3f;  // speed multiplier when wet

        [Header("Harvest event")]
        [SerializeField] private CrossObjectEventWithDataSO broadcastFarmingExpObject;
        
        private PlantStage currentStage = PlantStage.Planted;
        private bool hasPlant = false;
        private bool isWatered = false;
        private int requiredWaterings = 0;
        private int currentWaterings = 0;
        private FarmingExpObject farmingExpObject;
        private int waterCount = 0;
        private bool playerIsOver = false;
        private float growthProgress = 0f;

        private Animator animator;
        private Transform player;
        private PlayerController playerController;

        public delegate void OnHarvest();
        public event OnHarvest HarvestEvent;

        void Awake() => this.animator = GetComponent<Animator>();
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            playerController = FindObjectOfType<PlayerController>();

            if (promptUI != null) promptUI.SetActive(false);
        }

        void Update()
        {
            HandleInput();
            UpdateGrowth();

            if (player != null)
                this.UpdatePrompt();
        }

        #region Input Handling
        void HandleInput()
        {
            if (!playerIsOver) return;

            if (Input.GetKeyDown(KeyCode.C))
            {
                if (hasPlant && currentStage >= PlantStage.Grown)
                    HarvestPlant();
                else if (!hasPlant)
                    OpenInventoryForSeed();
            }

            if (Input.GetKeyDown(KeyCode.F) && hasPlant)
                WaterPlant();
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

            // Determine if watering should block growth (only for Planted and Seedling)
            bool stageRequiresWatering = (currentStage == PlantStage.Planted || currentStage == PlantStage.Seedling)
                                        && requiredWaterings > 0;

            if (stageRequiresWatering)
            {
                // Calculate how much of this stage can be grown based on watering
                float stageStart = currentStage == PlantStage.Planted ? 0f : 0.25f;
                float stageEnd = currentStage == PlantStage.Planted ? 0.25f : 0.5f;

                float wateringRatio = Mathf.Clamp01((float)currentWaterings / requiredWaterings);
                float stageCap = Mathf.Lerp(stageStart, stageEnd, wateringRatio);

                if (growthProgress >= stageCap)
                {
                    if (isWatered)
                    {
                        isWatered = false;
                        StopAllCoroutines(); // stop watering coroutine if running
                        PlayPlantAnimation("dry");
                    }

                    growthBar.color = Color.cyan; // waiting for more watering
                    growthBar.fillAmount = growthProgress;
                    return; // stop growth for this frame
                }
            }

            // Normal growth
            float growthThisFrame = baseGrowthSpeed * Time.deltaTime;
            if (isWatered) growthThisFrame *= wetGrowthMultiplier;

            growthProgress += growthThisFrame;
            growthProgress = Mathf.Clamp01(growthProgress);

            growthBar.fillAmount = growthProgress;
            UpdateGrowthBarColor();

            // Update stage after applying watering check
            PlantStage previousStage = currentStage;
            UpdatePlantStageByProgress();

            // Reset waterings when advancing a stage that still requires watering
            if (currentStage != previousStage && currentStage <= PlantStage.Seedling)
            {
                currentWaterings = 0;
            }
        }
        void UpdatePlantStageByProgress()
        {
            if (growthProgress < 0.25f) currentStage = PlantStage.Planted;
            else if (growthProgress < 0.50f) currentStage = PlantStage.Seedling;
            else if (growthProgress < 0.75f) currentStage = PlantStage.Grown;
            else if (growthProgress < 1f) currentStage = PlantStage.Wilting;
            else currentStage = PlantStage.Wilted;

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
            if (other.transform.root.CompareTag("Player"))
            {
                this.playerIsOver = true;
            }

            Debug.Log($"Player is over soil: {playerIsOver}");
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.transform.root.CompareTag("Player"))
                playerIsOver = false;
                if (promptUI != null) promptUI.SetActive(false);

            Debug.Log($"Player is over soil: {playerIsOver}");
        }
        #endregion

        #region Prompt System
        private void UpdatePrompt()
        {
            if (promptUI == null || promptText == null || player == null) return;

            // Calculate distance to player
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance > promptDistance)
            {
                if (promptUI.activeSelf) promptUI.SetActive(false);
                return;
            }

            // Show prompt
            if (!promptUI.activeSelf) promptUI.SetActive(true);

            // Position prompt above soil
            Vector3 targetPos = transform.position + Vector3.up * 1.5f;
            promptUI.transform.position = targetPos;

            // Set text based on plant state
            if (!hasPlant)
            {
                promptText.text = "Press <b>C</b> to Plant";
            }
            else if (currentStage >= PlantStage.Grown)
            {
                string text = "Press <b>C</b> to Harvest";
                if (!isWatered && currentStage <= PlantStage.Wilted)
                    text += "\nPress <b>F</b> to Water";
                promptText.text = text;
            }
            else
            {
                promptText.text = !isWatered ? "Press <b>F</b> to Water" : "";
            }
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
            currentWaterings = 0;

            if (ItemDatabase.TryGet(seedId, out ItemData itemData))
            {
                Game.Items.Plantable plantable = null;

                if (itemData.Properties != null)
                {
                    foreach (var prop in itemData.Properties)
                    {
                        if (prop is Game.Items.Plantable p)
                        {
                            plantable = p;
                            break;
                        }
                    }
                }
                
                if (plantable != null)
                {
                    float duration = Mathf.Max(1f, plantable.GrowthDuration);
                    this.baseGrowthSpeed = 1f / duration;
                    this.requiredWaterings = plantable.WateringRequirement;
                    farmingExpObject = plantable.FarmingExpObject;

                    Debug.Log($"🌱 Planted {seedId}: duration = {duration}s, requires {this.requiredWaterings} waterings.");
                }
                else
                {
                    Debug.LogWarning($"Seed {seedId} has no Plantable property.");
                    this.requiredWaterings = 0;
                    this.baseGrowthSpeed = 1f / 60f;
                }
            }
            else
            {
                Debug.LogWarning($"Seed item {seedId} not found in database.");
            }

            PlayPlantAnimation("dry");
        }

        void WaterPlant()
        {
            if (!hasPlant) return;
            if (isWatered) return;

            isWatered = true;
            waterCount++;
            currentWaterings++;

            Debug.Log($"💧 Watered {plantedSeedId} ({currentWaterings}/{requiredWaterings})");

            StartCoroutine(PlayPlantWateringThenWet());
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

            int cropCount = 1;
            int seedCount = 0;
            string dropItemId = plantedSeedId;

            switch (currentStage)
            {
                case PlantStage.Grown:
                    dropItemId = plantedSeedId.Replace("seed", "crop");
                    DropItem(dropItemId, cropCount);
                    seedCount = Random.Range(1, 3);
                    broadcastFarmingExpObject.TriggerEvent(this, farmingExpObject);
                    break;
                case PlantStage.Wilting:
                    dropItemId = plantedSeedId.Replace("seed", "wilting");
                    DropItem(dropItemId, cropCount);
                    seedCount = Random.Range(0, 2);
                    broadcastFarmingExpObject.TriggerEvent(this, farmingExpObject);
                    break;
                case PlantStage.Wilted:
                    seedCount = Random.Range(0, 2);
                    cropCount = 0;
                    break;
            }

            if (seedCount > 0) DropItem(plantedSeedId, seedCount);

            // Reset plant
            string harvestedSeedId = plantedSeedId;
            plantedSeedId = null;
            hasPlant = false;
            currentStage = PlantStage.Planted;
            isWatered = false;
            growthProgress = 0f;
            StopAllCoroutines(); 

            if (animator != null)
            {
                animator.SetTrigger("Harvest");
                animator.Play("dry_dirt", 0);
            }

            Debug.Log($"Player has auto replant: {playerController.HasAutoReplant}");
            if (playerController != null && playerController.HasAutoReplant)
                TryAutoReplant(harvestedSeedId);
        }

        private void TryAutoReplant(string seedId)
        {
            if (string.IsNullOrEmpty(seedId) || playerController == null) return;

            Inventory inventory = playerController.GetComponent<PlayerController>().GetComponentInChildren<Inventory>();
            if (inventory == null)
            {
                Debug.LogWarning("⚠️ Player inventory not found for auto replant.");
                return;
            }

            if (ItemDatabase.TryGet(seedId, out ItemData data))
            {
                ItemKey seedKey = ItemKey.From(data);

                if (inventory.Count(seedKey) > 0)
                {
                    // Consume 1 seed and replant
                    inventory.Remove(seedKey);
                    Debug.Log($"🌱 Auto replanted {seedId} from inventory.");
                    PlantSeed(seedId);
                }
                else
                {
                    Debug.Log($"⚠️ Auto replant failed — no {seedId} left in inventory.");
                }
            }

            
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
}
