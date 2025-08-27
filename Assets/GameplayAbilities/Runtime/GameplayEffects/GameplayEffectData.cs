using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects.Executions;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GameplayAbilities.Runtime.GameplayEffects {
    [Serializable]
    public class GameplayEffectData : IComparable<GameplayEffectData> {
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
        
        /// <summary>
        /// Checks if <paramref name="instigator"/> can afford to commit this effect.
        /// </summary>
        /// <param name="instigator">The actor that is committing the effect.</param>
        /// <returns><c>true</c> if all costs can be satisfied, <c>false</c> otherwise.</returns>
        internal bool CanCommit(AttributeSet instigator) {
            return this.Costs.TrueForAll(cost => cost.IsAffordable(instigator));
        }

        public GameplayEffect Instantiate(AttributeSet target, GameplayEffectExecutionArgs args) {
            return new GameplayEffect(this, args);
        }
        
        public override string ToString() {
            StringBuilder sb = new StringBuilder("Effect:\n");
            sb.AppendLine(this.ExecutionTime.ToString());
            switch (this.ExecutionTime) {
                case Periodicity.Periodic:
                    sb.AppendLine(this.Period.ToString(CultureInfo.InvariantCulture));
                    sb.AppendLine(this.Duration.ToString(CultureInfo.InvariantCulture));
                    break;
                case Periodicity.Continuous:
                    sb.AppendLine(this.Duration.ToString(CultureInfo.InvariantCulture));
                    break;
            }
            
            sb.AppendLine($"Can miss: {this.CanMiss}");
            sb.AppendLine($"Success chance: {this.BaseChance}%");
            foreach (EffectCommitmentCost cost in this.Costs.OrderBy(cost => cost)) {
                sb.AppendLine(cost.ToString());
            }

            return sb.AppendLine(this.Executor.ToString()).ToString();
        }

        public int CompareTo(GameplayEffectData other) {
            if (other is null) {
                return 1;
            }
            
            return object.ReferenceEquals(this, other) ? 0 : string.CompareOrdinal(this.ToString(), other.ToString());
        }
    }
}
