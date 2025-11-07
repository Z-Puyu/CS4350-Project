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
using Game.Player;
using Inventory_related.Inventory_UI_Manager_V2;

namespace Farming_related {
    public class SoilPlantInteraction : MonoBehaviour
    {
        public enum PlantStage { Planted, Seedling, Grown, Wilting, Wilted }

        [Header("UI")]
        [Tooltip("Floating TMP text for the prompt. Assign the TextMeshPro object directly.")]
        [SerializeField] private TMP_Text promptText;
        [SerializeField] private float promptDistance = 2f; // distance at which popup appears

        [Header("Plant Info")]
        [SerializeField] private string plantedSeedId; // which seed type (from inventory)
        [SerializeField] private PickUp2D pickUpPrefab;
    
        [Header("Growth Settings")]
        [SerializeField] private Image growthBar;
        [SerializeField] private float baseGrowthSpeed = 0.01f;   // per second when dry
        [SerializeField] private float wetGrowthMultiplier = 3f;  // speed multiplier when wet
        
        [Header("Stage Thresholds (0-1)")]
        [SerializeField, Range(0f, 1f)] private float plantedThreshold = 0.25f;
        [SerializeField, Range(0f, 1f)] private float seedlingThreshold = 0.50f;
        [SerializeField, Range(0f, 1f)] private float grownThreshold = 0.75f;
        [SerializeField, Range(0f, 1f)] private float wiltingThreshold = 1.0f;

        [Header("Buff Settings")]
        [Tooltip("Normalized progress when decay-based plant buff begins (0-1)")]
        [SerializeField, Range(0f,1f)] private float buffApplyStart = 0.5f;
        [Tooltip("Normalized progress when decay-based plant buff ends (exclusive)")]
        [SerializeField, Range(0f,1f)] private float buffApplyEnd = 1.0f;

        [Header("Harvest event")]
        [SerializeField] private CrossObjectEventWithDataSO broadcastFarmingExpObject;
        
    private PlantStage currentStage = PlantStage.Planted;
    private FarmingExpObject farmingExpObject;
    private bool playerIsOver = false;

    // Growth controller lives on the same GameObject and owns growth state
    private PlantGrowthController growthController;

        private Animator animator;
        private Transform player;
        private PlayerController playerController;
    // persisted plantable for buff values
    private Game.Items.Plantable currentPlantable;
    // player's AttributeSet to apply global buff to (kept here so refs can be passed to the provider)
    private GameplayAbilities.Runtime.Attributes.AttributeSet playerAttributeSet;
    // external provider that owns buff logic for this plant
    private PlantBuffProvider plantBuffProvider;
    // controller that owns harvesting/drop/auto-replant behavior
    private PlantHarvestController harvestController;

        public delegate void OnHarvest();
        public event OnHarvest HarvestEvent;


        void Awake() => this.animator = GetComponent<Animator>();
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            playerController = FindObjectOfType<PlayerController>();

            if (promptText != null) promptText.gameObject.SetActive(false);

            if (player != null) {
                playerAttributeSet = player.GetComponentInChildren<GameplayAbilities.Runtime.Attributes.AttributeSet>();
            }

            // ensure growth controller exists
            growthController = GetComponent<PlantGrowthController>();
            if (growthController == null) growthController = gameObject.AddComponent<PlantGrowthController>();
            // subscribe to growth events for UI/animation updates
            growthController.OnProgressChanged += (p) => {
                if (growthBar != null) {
                    growthBar.fillAmount = p;
                    UpdateGrowthBarColor(p);
                }
            };
            growthController.OnStageChanged += (prev, next) => {
                currentStage = next;
                // trigger animation update
                PlayPlantAnimation(growthController.IsWatered ? "wet" : "dry");
                // inform buff provider
                plantBuffProvider?.ApplyStageBuff(currentStage);
            };
            growthController.OnWaterStateChanged += (isWet) => {
                // update animation when watered state changes
                PlayPlantAnimation(isWet ? "wet" : "dry");
            };

            // ensure harvest controller exists (harvest logic lives in its own component)
            harvestController = GetComponent<PlantHarvestController>();
            if (harvestController == null) harvestController = gameObject.AddComponent<PlantHarvestController>();
        }

        void Update()
        {
            HandleInput();
            // keep buff provider in sync using controller progress
            plantBuffProvider?.UpdateBuff(growthController != null ? growthController.GrowthProgress : 0f);

            // local UI prompt

            if (player != null)
                this.UpdatePrompt();
        }

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
            {
                this.playerIsOver = false;
                if (promptText != null) promptText.gameObject.SetActive(false);
            }

            Debug.Log($"Player is over soil: {playerIsOver}");
        }
        #endregion

        #region Input Handling
        void HandleInput()
        {
            if (!playerIsOver) return;

            if (Input.GetKeyDown(KeyCode.C))
            {
                if (growthController != null && growthController.HasPlant && growthController.CurrentStage >= PlantStage.Grown)
                    HarvestPlant();
                else if (growthController == null || !growthController.HasPlant)
                    OpenInventoryForSeed();
            }

            if (Input.GetKeyDown(KeyCode.F) && growthController != null && growthController.HasPlant)
                WaterPlant();
        }
        #endregion

        #region Growth System
        // Growth is handled by PlantGrowthController; UI updates are subscribed in Start
        #endregion

        // Update growth bar color using a given progress value
        private void UpdateGrowthBarColor(float progress)
        {
            if (growthBar == null) return;
            if (progress < 0.5f) growthBar.color = Color.green;
            else if (progress < 0.75f) growthBar.color = new Color(0.6f, 0f, 0.6f); // purple
            else growthBar.color = Color.red;
        }

        #region Prompt System
        private void UpdatePrompt()
        {
            if (promptText == null || player == null) return;

            // Calculate distance to player
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance > promptDistance)
            {
                if (promptText.gameObject.activeSelf) promptText.gameObject.SetActive(false);
                return;
            }

            // Show prompt
            if (!promptText.gameObject.activeSelf) promptText.gameObject.SetActive(true);

            // Position prompt above soil
            Vector3 targetPos = transform.position + Vector3.up * 1.5f;
            promptText.transform.position = targetPos;

            // Set text based on plant state
            if (growthController == null || !growthController.HasPlant)
            {
                promptText.text = "Press <b>C</b> to Plant";
            }
            else if (currentStage >= PlantStage.Grown)
            {
                string text = "Press <b>C</b> to Harvest";
                if (!growthController.IsWatered && currentStage <= PlantStage.Wilted)
                    text += "\nPress <b>F</b> to Water";
                promptText.text = text;
            }
            else
            {
                promptText.text = !growthController.IsWatered ? "Press <b>F</b> to Water" : "";
            }
        }
        #endregion

        #region Planting & Watering
        void OpenInventoryForSeed()
        {
            InventoryUIManagerV2.Instance.OpenForSeedSelection(this);
        }

        public void PlantSeed(string seedId)
        {
            // ensure player references are available before we try to apply buffs
            EnsurePlayerRefs();

            plantedSeedId = seedId;
            // ensure growth controller exists (Start also creates it, but be defensive)
            if (growthController == null) growthController = GetComponent<PlantGrowthController>() ?? gameObject.AddComponent<PlantGrowthController>();

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
                    // persist plantable so we can read AttackBuff later
                    this.currentPlantable = plantable;
                    float duration = Mathf.Max(1f, plantable.GrowthDuration);
                    // initialize growth controller with plant parameters and thresholds
                    growthController.Initialize(duration, plantable.WateringRequirement, wetGrowthMultiplier, plantedThreshold, seedlingThreshold, grownThreshold, wiltingThreshold);
                    growthController.Plant();
                    farmingExpObject = plantable.FarmingExpObject;

                    Debug.Log($"🌱 Planted {seedId}: duration = {duration}s, requires {plantable.WateringRequirement} waterings.");
                }
                else
                {
                    Debug.LogWarning($"Seed {seedId} has no Plantable property.");
                }
            }
            else
            {
                Debug.LogWarning($"Seed item {seedId} not found in database.");
            }

            PlayPlantAnimation("dry");
            // Initialize buff state for newly planted seed
            // create or fetch provider and initialize it with the plantable & player AttributeSet
            plantBuffProvider = GetComponent<PlantBuffProvider>();
            if (plantBuffProvider == null) plantBuffProvider = gameObject.AddComponent<PlantBuffProvider>();
            // ensure we have player refs available for the provider
            EnsurePlayerRefs();
            plantBuffProvider.Initialize(this.currentPlantable, playerAttributeSet);
            plantBuffProvider.SetBuffRange(buffApplyStart, buffApplyEnd);
            plantBuffProvider.ApplyStageBuff(currentStage);

            // wire harvest controller with the same references so it can perform drops/auto-replant
            if (harvestController == null) harvestController = GetComponent<PlantHarvestController>() ?? gameObject.AddComponent<PlantHarvestController>();
            harvestController.SetReferences(this, growthController, plantBuffProvider, animator, playerController, pickUpPrefab, broadcastFarmingExpObject);
        }

        // Ensure player transform and attribute set are cached (recovery helper)
        private void EnsurePlayerRefs()
        {
            // Try to use the same root-based lookup pattern as auto-replant: prefer the PlayerController root
            if (playerController == null)
            {
                playerController = FindObjectOfType<PlayerController>();
            }

            if (player == null)
            {
                var p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) player = p.transform;
            }

            if (playerAttributeSet == null)
            {
                // 1) Try to find AttributeSet under the PlayerController root (preferred)
                if (playerController != null)
                {
                    var root = playerController.transform.root;
                    if (root != null)
                    {
                        playerAttributeSet = root.GetComponentInChildren<GameplayAbilities.Runtime.Attributes.AttributeSet>(true);
                    }
                }

                // 2) Fallback: find under the player transform
                if (playerAttributeSet == null && player != null)
                {
                    playerAttributeSet = player.GetComponentInChildren<GameplayAbilities.Runtime.Attributes.AttributeSet>(true);
                }
            }

            Debug.Log($"[PlantBuff] EnsurePlayerRefs: player={(player!=null?player.name:"null")}, playerController={(playerController!=null?playerController.name:"null")}, playerAttributeSet={(playerAttributeSet!=null?playerAttributeSet.gameObject.name:"null")} ");
        }

        void WaterPlant()
        {
            if (growthController == null || !growthController.HasPlant) return;
            growthController.WaterPlant();
            Debug.Log($"💧 Watered {plantedSeedId}");
        }
            // update global player buff from plant (applies regardless of player location)
            // (moved to coroutine and UpdateGrowth to avoid stray top-level calls)
        #endregion

        #region Harvest
        void HarvestPlant()
        {
            // Keep the HarvestEvent invocation on the SoilPlantInteraction so
            // external listeners (subscribers) remain unchanged.
            HarvestEvent?.Invoke();

            // Delegate harvest logic to the dedicated controller. The controller
            // will return true if it performed an auto-replant.
            bool replanted = false;
            if (harvestController != null)
            {
                replanted = harvestController.Harvest(plantedSeedId, currentStage, farmingExpObject);
            }

            // If no auto-replant occurred, clear the planted seed
            if (!replanted)
            {
                plantedSeedId = null;
            }
        }

        #endregion

        // Buff behaviour is now delegated to PlantBuffProvider component so the
        // buff system is separated from the SoilPlantInteraction responsibilities.

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
