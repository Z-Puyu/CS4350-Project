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
        internal enum Periodicity { Instant, Periodic, Continuous }
        
        [field: SerializeField, LayoutStart("Duration", ELayout.Horizontal)] 
        internal Periodicity ExecutionTime { get; private set; } = Periodicity.Instant;
        
        [field: SerializeField, ShowIf(nameof(this.ExecutionTime), Periodicity.Periodic)]
        [field: PostFieldRichLabel("<color=grey>s between ticks")]
        internal float Period { get; private set; } = 1f;

        [field: SerializeField, HideIf(nameof(this.ExecutionTime), Periodicity.Instant)]
        [field: Tooltip("Set to -1 for infinite duration"), PostFieldRichLabel("<color=grey>s"), LayoutEnd]
        internal float Duration { get; private set; } = 1f;
        
        [field: SerializeField, PropRange(0, 100)] 
        internal int BaseChance { get; private set; } = 100;
        
        [field: SerializeField] internal List<EffectCommitmentCost> Costs { get; private set; } = new List<EffectCommitmentCost>();
        [field: SerializeReference] internal EffectExecution Executor { get; private set; } = new ModifierEffectExecution();
        [field: SerializeField] internal bool HasLevel { get; set; }

        [field: SerializeField, Tooltip("This curve describes how a level is mapped to a modifier value coefficient")]
        [field: ShowIf(nameof(this.HasLevel))]
        internal AnimationCurve LevelEffect { get; set; } = AnimationCurve.Linear(-1, -1, 1, 1);
        
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
