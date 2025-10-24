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
    // persisted plantable for buff values
    private Game.Items.Plantable currentPlantable;
    // player's AttributeSet to apply global buff to
    private GameplayAbilities.Runtime.Attributes.AttributeSet playerAttributeSet;
    // active modifier applied to player (so we can remove it later)
    private GameplayAbilities.Runtime.Modifiers.Modifier activePlayerModifier = GameplayAbilities.Runtime.Modifiers.Modifier.Empty;

        public delegate void OnHarvest();
        public event OnHarvest HarvestEvent;


        void Awake() => this.animator = GetComponent<Animator>();
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            playerController = FindObjectOfType<PlayerController>();

            if (promptUI != null) promptUI.SetActive(false);

            if (player != null) {
                playerAttributeSet = player.GetComponentInChildren<GameplayAbilities.Runtime.Attributes.AttributeSet>();
            }
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

            // If stage changed, apply/remove flat buff based on new stage
            if (currentStage != previousStage)
            {
                ApplyStageBuff(currentStage);
            }
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
            // ensure player references are available before we try to apply buffs
            EnsurePlayerRefs();

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
                    // persist plantable so we can read AttackBuff later
                    this.currentPlantable = plantable;
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
            // Initialize buff state for newly planted seed
            ApplyStageBuff(currentStage);
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
            // update global player buff from plant (applies regardless of player location)
            // (moved to coroutine and UpdateGrowth to avoid stray top-level calls)
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
            // remove any active buff when harvested
            RemovePlayerBuff();
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
            Debug.Log("Attempting auto replant...");

            if (string.IsNullOrEmpty(seedId) || playerController == null)
            {
                Debug.LogWarning("⚠️ Cannot auto replant due to missing seedId or playerController.");
                return;
            }

            // Strict lookup: only use the Inventory located under the Player root.
            Inventory inventory = null;
            Transform root = playerController.transform.root;
            if (root != null)
            {
                inventory = root.GetComponentInChildren<Inventory>(true);
            }

            if (inventory == null)
            {
                Debug.LogWarning("⚠️ Player inventory not found under Player root — auto replant cancelled.");
                return;
            }
                // Remove player buff on harvest reset
                // RemovePlayerBuff(); // This line is removed as per the patch goal

            Debug.Log($"Using Inventory '{inventory.gameObject.name}' for auto replant. Checking inventory for seed {seedId}...");

            if (ItemDatabase.TryGet(seedId, out ItemData data))
            {
                ItemKey seedKey = ItemKey.From(data);

                int count = inventory.Count(seedKey);
                Debug.Log($"[Replant] seedKey='{seedKey}', seedId='{seedId}', count={count}");
                if (count > 0)
                {
                    inventory.Remove(seedKey);
                    Debug.Log($"🌱 Auto replanted {seedId} from inventory (removed key '{seedKey}').");
                    PlantSeed(seedId);
                }
                else
                {
                    Debug.LogWarning($"⚠️ Auto replant failed — no item for key '{seedKey}' in this inventory.");
                    // dump inventory contents for diagnosis using the public enumerator
                    Debug.Log("Inventory contents:");
                    foreach (var kv in inventory) // inventory implements IEnumerable<KeyValuePair<ItemKey,int>>
                    {
                        ItemKey key = kv.Key;
                        int qty = kv.Value;
                        Debug.Log($" - id='{key.Id}', qty={qty}");
                    }
                }
                // ...existing code...
            }
            else
            {
                Debug.LogWarning($"Seed item {seedId} not found in database for auto replant.");
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

        // ---------------------- Plant buff application to player ----------------------
        // These are class-level methods (previously were accidentally nested inside DropItem
        // which caused parse errors). The buff is applied when the plant is between
        // Grown (inclusive) and Wilting (exclusive) and decays linearly.
        private void UpdatePlayerBuff()
        {
            if (!hasPlant || currentPlantable == null || playerAttributeSet == null) {
                RemovePlayerBuff();
                return;
            }

            if (growthProgress < 0.5f || growthProgress >= 1.0f) {
                RemovePlayerBuff();
                return;
            }

            float normalized = (growthProgress - 0.5f) / 0.5f;
            float buffPercent = Mathf.Clamp01(1f - normalized);
            int magnitude = Mathf.RoundToInt(currentPlantable.AttackBuff * buffPercent);

            if (magnitude <= 0) {
                RemovePlayerBuff();
                return;
            }

            var modifier = new GameplayAbilities.Runtime.Modifiers.Modifier(magnitude, GameplayAbilities.Runtime.Modifiers.Modifier.Operation.Multiply, "Damage.Physical");
            if (!activePlayerModifier.Equals(GameplayAbilities.Runtime.Modifiers.Modifier.Empty) && activePlayerModifier.Equals(modifier)) return;

            RemovePlayerBuff();
            playerAttributeSet.AddModifier(modifier);
            activePlayerModifier = modifier;
            Debug.Log($"[PlantBuff] Applied player buff +{magnitude}% to Damage.Physical (seed={plantedSeedId}, progress={growthProgress:F2})");
        }

        private void RemovePlayerBuff()
        {
            if (activePlayerModifier.Equals(GameplayAbilities.Runtime.Modifiers.Modifier.Empty)) return;
            if (playerAttributeSet == null) { activePlayerModifier = GameplayAbilities.Runtime.Modifiers.Modifier.Empty; return; }

            playerAttributeSet.AddModifier(-activePlayerModifier);
            activePlayerModifier = GameplayAbilities.Runtime.Modifiers.Modifier.Empty;
            Debug.Log("[PlantBuff] Removed player buff");
        }

        // Apply a flat buff based on discrete plant stage transitions:
        // - Grown  => full AttackBuff
        // - Wilting => half AttackBuff
        // - Wilted/others => remove buff
        private void ApplyStageBuff(PlantStage stage)
        {
            Debug.Log($"[PlantBuff] ApplyStageBuff called: stage={stage}, plantedSeedId={plantedSeedId}, hasPlant={hasPlant}, currentPlantable={(currentPlantable!=null?"yes":"no")}, playerAttributeSet={(playerAttributeSet!=null?playerAttributeSet.gameObject.name:"null")} ");

            // Attempt to recover missing playerAttributeSet at runtime
            if (playerAttributeSet == null)
            {
                var playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    playerAttributeSet = playerObj.GetComponentInChildren<GameplayAbilities.Runtime.Attributes.AttributeSet>();
                }
                Debug.Log($"[PlantBuff] Re-fetched playerAttributeSet={(playerAttributeSet!=null?playerAttributeSet.gameObject.name:"null")}");
            }

            // Attempt to recover currentPlantable from the plantedSeedId if possible
            if (currentPlantable == null && !string.IsNullOrEmpty(plantedSeedId))
            {
                if (ItemDatabase.TryGet(plantedSeedId, out ItemData data) && data.Properties != null)
                {
                    foreach (var prop in data.Properties)
                    {
                        if (prop is Game.Items.Plantable p)
                        {
                            currentPlantable = p;
                            break;
                        }
                    }
                }
                Debug.Log($"[PlantBuff] Recovered currentPlantable={(currentPlantable!=null?"yes":"no")} from plantedSeedId={plantedSeedId}");
            }

            if (currentPlantable == null || playerAttributeSet == null)
            {
                Debug.Log("[PlantBuff] ApplyStageBuff aborted: missing currentPlantable or playerAttributeSet after recovery attempts");
                RemovePlayerBuff();
                return;
            }

            int baseBuff = (int) Mathf.Max(0, currentPlantable.AttackBuff);

            switch (stage)
            {
                case PlantStage.Grown:
                    {
                        var mod = new GameplayAbilities.Runtime.Modifiers.Modifier(baseBuff, GameplayAbilities.Runtime.Modifiers.Modifier.Operation.Multiply, "Damage.Physical");
                        RemovePlayerBuff();
                        playerAttributeSet.AddModifier(mod);
                        activePlayerModifier = mod;
                        Debug.Log($"[PlantBuff] Stage Grown: Applied flat buff +{baseBuff}% to Damage.Physical");
                        break;
                    }
                case PlantStage.Wilting:
                    {
                        int half = Mathf.RoundToInt(baseBuff * 0.5f);
                        if (half <= 0) { RemovePlayerBuff(); break; }
                        var mod = new GameplayAbilities.Runtime.Modifiers.Modifier(half, GameplayAbilities.Runtime.Modifiers.Modifier.Operation.Multiply, "Damage.Physical");
                        RemovePlayerBuff();
                        playerAttributeSet.AddModifier(mod);
                        activePlayerModifier = mod;
                        Debug.Log($"[PlantBuff] Stage Wilting: Applied flat buff +{half}% (half) to Damage.Physical");
                        break;
                    }
                default:
                    RemovePlayerBuff();
                    break;
            }
        }

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
