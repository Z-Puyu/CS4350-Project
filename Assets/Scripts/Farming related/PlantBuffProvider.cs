using UnityEngine;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Modifiers;

namespace Farming_related
{
    /// <summary>
    /// Component that owns plant -> player buff logic.
    /// Extracted from SoilPlantInteraction so buff responsibilities are isolated.
    /// </summary>
    public class PlantBuffProvider : MonoBehaviour
    {
        private Game.Items.Plantable plantable;
        private AttributeSet playerAttributeSet;
        private Modifier activeModifier = Modifier.Empty;
        // configurable decay-based buff range (normalized progress)
        private float buffStart = 0.5f;
        private float buffEnd = 1.0f;

        public void Initialize(Game.Items.Plantable plantable, AttributeSet attributeSet)
        {
            this.plantable = plantable;
            this.playerAttributeSet = attributeSet;
        }

        /// <summary>
        /// Configure the normalized progress range where the decay-based buff is active.
        /// </summary>
        public void SetBuffRange(float start, float end)
        {
            buffStart = Mathf.Clamp01(start);
            buffEnd = Mathf.Clamp01(end);
            if (buffEnd < buffStart) buffEnd = buffStart;
        }

        /// <summary>
        /// Call each frame (or when growth changes) to apply/remove a decaying buff
        /// based on growth progress. This mirrors the original behaviour which
        /// applied a percent buff between 50% and 100% growth.
        /// </summary>
        public void UpdateBuff(float growthProgress)
        {
            if (plantable == null || playerAttributeSet == null)
            {
                RemoveBuff();
                return;
            }

            if (growthProgress < buffStart || growthProgress >= buffEnd)
            {
                RemoveBuff();
                return;
            }

            float denom = Mathf.Max(0.0001f, buffEnd - buffStart);
            float normalized = (growthProgress - buffStart) / denom;
            float buffPercent = Mathf.Clamp01(1f - normalized);
            int magnitude = Mathf.RoundToInt(plantable.AttackBuff * buffPercent);

            if (magnitude <= 0)
            {
                RemoveBuff();
                return;
            }

            var targetAttribute = plantable.BuffAttribute == null ? "Damage.Physical" : plantable.BuffAttribute.Id;
            var modifier = new Modifier(magnitude, Modifier.Operation.Multiply, targetAttribute);
            if (!activeModifier.Equals(Modifier.Empty) && activeModifier.Equals(modifier)) return;

            RemoveBuff();
            playerAttributeSet.AddModifier(modifier);
            activeModifier = modifier;
            Debug.Log($"[PlantBuffProvider] Applied player buff +{magnitude}% to {targetAttribute} (seed, progress={growthProgress:F2})");
        }

        /// <summary>
        /// Apply a flat buff based on discrete plant stage transitions:
        /// - Grown  => full AttackBuff
        /// - Wilting => half AttackBuff
        /// - Wilted/others => remove buff
        /// </summary>
        public void ApplyStageBuff(SoilPlantInteraction.PlantStage stage)
        {
            if (plantable == null)
            {
                RemoveBuff();
                return;
            }

            int baseBuff = (int) Mathf.Max(0, plantable.AttackBuff);

            switch (stage)
            {
                case SoilPlantInteraction.PlantStage.Grown:
                    {
                        var targetAttribute = plantable.BuffAttribute == null ? "Damage.Physical" : plantable.BuffAttribute.Id;
                        var mod = new Modifier(baseBuff, Modifier.Operation.Multiply, targetAttribute);
                        RemoveBuff();
                        if (playerAttributeSet != null)
                        {
                            playerAttributeSet.AddModifier(mod);
                            activeModifier = mod;
                            Debug.Log($"[PlantBuffProvider] Stage Grown: Applied flat buff +{baseBuff}% to {targetAttribute}");
                        }
                        break;
                    }
                case SoilPlantInteraction.PlantStage.Wilting:
                    {
                        int half = Mathf.RoundToInt(baseBuff * 0.5f);
                        if (half <= 0) { RemoveBuff(); break; }
                        var targetAttribute = plantable.BuffAttribute == null ? "Damage.Physical" : plantable.BuffAttribute.Id;
                        var mod = new Modifier(half, Modifier.Operation.Multiply, targetAttribute);
                        RemoveBuff();
                        if (playerAttributeSet != null)
                        {
                            playerAttributeSet.AddModifier(mod);
                            activeModifier = mod;
                            Debug.Log($"[PlantBuffProvider] Stage Wilting: Applied flat buff +{half}% (half) to {targetAttribute}");
                        }
                        break;
                    }
                default:
                    RemoveBuff();
                    break;
            }
        }

        public void RemoveBuff()
        {
            if (activeModifier.Equals(Modifier.Empty)) return;
            if (playerAttributeSet == null) { activeModifier = Modifier.Empty; return; }

            playerAttributeSet.AddModifier(-activeModifier);
            activeModifier = Modifier.Empty;
            Debug.Log("[PlantBuffProvider] Removed player buff");
        }

        // Optional helpers for dynamic updates
        public void SetPlantable(Game.Items.Plantable p) => plantable = p;
        public void SetAttributeSet(AttributeSet s) => playerAttributeSet = s;
    }
}
