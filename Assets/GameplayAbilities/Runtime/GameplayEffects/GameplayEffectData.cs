using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects.Executions;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.GameplayEffects {
    [Serializable]
    public sealed class GameplayEffectData : IComparable<GameplayEffectData>, IEquatable<GameplayEffectData> {
        private Lazy<string> CachedSortKey { get; }
        public string SortKey => this.CachedSortKey.Value;

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

        private GameplayEffectData() {
            this.CachedSortKey = new Lazy<string>(this.GenerateSortKey);
        }
        
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
        
        public string GenerateSortKey() {
            StringBuilder sb = new StringBuilder(this.GetType().FullName);
            sb.Append($"_ExecutionTime:{this.ExecutionTime}");
            switch (this.ExecutionTime) {
                case Periodicity.Periodic:
                    sb.Append($"_Period:{this.Period}");
                    sb.Append($"_Duration:{this.Duration}");
                    break;
                case Periodicity.Continuous:
                    sb.Append($"_Duration:{this.Duration}");
                    break;
            }
            
            sb.Append($"_CanMiss:{this.CanMiss}");
            sb.Append($"_BaseChance: {this.BaseChance}%");
            List<EffectCommitmentCost> costs = this.Costs.ToList();
            costs.Sort();
            foreach (EffectCommitmentCost cost in costs) {
                sb.Append($"_Cost:{cost.SortKey}");
            }

            return sb.Append($"_Executor:{this.Executor.SortKey}").ToString();
        }

        public int CompareTo(GameplayEffectData other) {
            if (other is null) {
                return 1;
            }

            return object.ReferenceEquals(this, other)
                    ? 0
                    : string.CompareOrdinal(this.CachedSortKey.Value, other.CachedSortKey.Value);
        }

        public bool Equals(GameplayEffectData other) {
            return this.CompareTo(other) == 0;
        }
        
        public override int GetHashCode() {
            return this.SortKey.GetHashCode();
        }
    }
}
