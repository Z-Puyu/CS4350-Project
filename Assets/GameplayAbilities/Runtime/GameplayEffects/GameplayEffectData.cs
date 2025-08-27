using System;
using System.Collections.Generic;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects.Executions;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GameplayAbilities.Runtime.GameplayEffects {
    [Serializable]
    public class GameplayEffectData {
        public enum Periodicity { Instant, Periodic, Continuous }
        
        [field: SerializeReference, DefaultExpand] 
        public EffectExecution Executor { get; private set; } = new ModifierEffectExecution();
        
        [field: SerializeField] public Periodicity ExecutionTime { get; private set; } = Periodicity.Instant;
        
        [field: SerializeField, ShowIf(nameof(this.ExecutionTime), Periodicity.Periodic)]
        [field: PostFieldRichLabel("<color=grey>s between ticks")]
        public float Period { get; private set; } = 1f;

        [field: SerializeField, HideIf(nameof(this.ExecutionTime), Periodicity.Instant)]
        [field: Tooltip("Set to -1 for infinite duration"), PostFieldRichLabel("<color=grey>s")]
        public float Duration { get; private set; } = 1f;
        
        [field: SerializeField, Tooltip("Whether this effect can fail to apply")]
        public bool CanMiss { get; private set; }
        
        [field: SerializeField, PropRange(0, 100), ShowIf(nameof(this.CanMiss))] 
        public int BaseChance { get; private set; } = 100;
        
        [field: SerializeField] public List<EffectCommitmentCost> Costs { get; private set; } = new List<EffectCommitmentCost>();
        [field: SerializeField] internal bool HasLevel { get; private set; }

        [field: SerializeField, Tooltip("This curve describes how a level is mapped to a modifier value coefficient")]
        [field: ShowIf(nameof(this.HasLevel))]
        internal AnimationCurve LevelEffect { get; private set; } = AnimationCurve.Linear(-1, -1, 1, 1);
        
        /// <summary>
        /// Checks if <paramref name="instigator"/> can afford to commit this effect.
        /// </summary>
        /// <param name="instigator">The actor that is committing the effect.</param>
        /// <returns><c>true</c> if all costs can be satisfied, <c>false</c> otherwise.</returns>
        internal bool CanCommit(AttributeSet instigator) {
            return this.Costs.TrueForAll(cost => cost.IsAffordable(instigator));
        }

        internal GameplayEffect Instantiate(AttributeSet target, GameplayEffectExecutionArgs args) {
            return new GameplayEffect(this, args);
        }
    }
}
