using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameplayAbilities.Runtime.GameplayEffects {
    [Serializable]
    public abstract class GameplayEffectData : IComparable<GameplayEffectData>, IEquatable<GameplayEffectData> {
        private Lazy<string> CachedSortKey { get; }
        public string SortKey => this.CachedSortKey.Value;

        public enum Periodicity { Instant, Periodic, Continuous }
        
        [field: SerializeField] public Periodicity ExecutionTime { get; private set; } = Periodicity.Instant;
        
        [field: SerializeField, ShowIf(nameof(this.ExecutionTime), Periodicity.Periodic)]
        [field: PostFieldRichLabel("<color=grey>s between ticks")]
        public float Period { get; private set; } = 1f;

        [field: SerializeField, HideIf(nameof(this.ExecutionTime), Periodicity.Instant)]
        [field: Tooltip("Set to -1 for infinite duration"), PostFieldRichLabel("<color=grey>s")]
        private int Duration { get; set; } = 1;
        
        [field: SerializeField, Tooltip("Whether this effect can fail to apply")]
        public bool CanMiss { get; private set; }
        
        [field: SerializeField, PropRange(0, 100), ShowIf(nameof(this.CanMiss))] 
        public int BaseChance { get; private set; } = 100;
        
        [field: SerializeField] public List<EffectCommitmentCost> Costs { get; private set; } = new List<EffectCommitmentCost>();

        public int ActualDuration => this.ExecutionTime switch {
            Periodicity.Instant => 0,
            Periodicity.Periodic or Periodicity.Continuous => this.Duration,
            var _ => throw new ArgumentOutOfRangeException()
        };
        
        public GameplayEffectData() {
            this.CachedSortKey = new Lazy<string>(this.GenerateSortKey);
        }

        public virtual DropdownList<string> GetDataLabels() {
            return new DropdownList<string>();
        }
        
        /// <summary>
        /// Additional logic to execute on the first execution of the gameplay effect.
        /// </summary>
        /// <param name="target">The target of the gameplay effect.</param>
        /// <param name="args">The execution arguments.</param>
        public virtual void OnFirstExecution(AttributeSet target, GameplayEffectExecutionArgs args) { }

        /// <summary>
        /// Run the main execution logic here.
        /// </summary>
        /// <param name="target">The target of the gameplay effect.</param>
        /// <param name="args">The additional arguments used to execute the gameplay effect.</param>
        /// <returns></returns>
        public abstract IEnumerable<Modifier> Run(AttributeSet target, GameplayEffectExecutionArgs args);
        
        /// <summary>
        /// Checks if the gameplay effect application to the target is successful.
        /// </summary>
        /// <param name="target">The target of the effect.</param>
        /// <param name="chance">The base probability of the effect being successfully applied.</param>
        /// <param name="args">The arguments used to invoke the gameplay effect.</param>
        /// <returns><c>true</c> if the effect si successfully applied; otherwise, <c>false</c>.</returns>
        /// <remarks>This is a good place to implement custom probability logic like precision or luck.</remarks>
        public virtual GameplayEffect.Outcome Try(IAttributeReader target, int chance, GameplayEffectExecutionArgs args) {
            bool isSuccess = chance switch {
                >= 100 => true,
                <= 0 => false,
                var _ => Random.Range(0, 100) < chance
            };
            
            return isSuccess ? GameplayEffect.Outcome.Success : GameplayEffect.Outcome.Failure;
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
        
        protected virtual string GenerateSortKey() {
            StringBuilder sb = new StringBuilder(this.GetType().FullName);
            sb.Append($"-ExecutionTime:{this.ExecutionTime}");
            switch (this.ExecutionTime) {
                case Periodicity.Periodic:
                    sb.Append($"-Period:{this.Period}");
                    sb.Append($"-Duration:{this.Duration}");
                    break;
                case Periodicity.Continuous:
                    sb.Append($"-Duration:{this.Duration}");
                    break;
            }
            
            sb.Append($"-CanMiss:{this.CanMiss}");
            sb.Append($"-BaseChance: {this.BaseChance}%");
            List<EffectCommitmentCost> costs = this.Costs.ToList();
            costs.Sort();
            foreach (EffectCommitmentCost cost in costs) {
                sb.Append($"-Cost:{cost.SortKey}");
            }

            return sb.ToString();
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
