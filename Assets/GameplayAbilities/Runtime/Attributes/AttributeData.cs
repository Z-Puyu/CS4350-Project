using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.ModificationRules;
using GameplayAbilities.Runtime.Modifiers;
using UnityEngine;

namespace GameplayAbilities.Runtime.Attributes {
    internal class AttributeData {
        internal enum ModifierMode { ByPriority, ByTimeOrder }
        
        private AttributeSet Root { get; }
        private List<IAttributeClampRule> ModificationRules { get; } = new List<IAttributeClampRule>();
        private float BaseValue { get; }
        private ModifierMode Mode { get; }
        internal int Value { get; private set; }
        
        internal int MaxValue => Math.Min(int.MaxValue, (int)this.ExecuteModificationRules(int.MaxValue));
        internal int MinValue => Math.Max(int.MinValue, (int)this.ExecuteModificationRules(int.MinValue));

        private SortedList<Modifier.Operation, Modifier> Modifiers { get; } =
            new SortedList<Modifier.Operation, Modifier>();
        
        private LinkedList<Modifier> ModifierSequence { get; } = new LinkedList<Modifier>();

        private AttributeData(float value, AttributeSet root, ModifierMode mode) {
            this.BaseValue = value;
            this.Root = root;
            this.Mode = mode;
        }

        internal static AttributeData From(
            AttributeType definition, float initValue, AttributeSet root, ModifierMode mode
        ) {
            AttributeData data = new AttributeData(initValue, root, mode);
            foreach (IAttributeClampRule rule in definition.ModificationRules) {
                data.ModificationRules.Add(rule);
            }
            
            data.Value = data.RecomputeValue();
            return data;
        }

        private double ExecuteModificationRules(double value) {
            foreach (IAttributeClampRule rule in this.ModificationRules) {
                int min = rule.MinValueIn(this.Root);
                int max = rule.MaxValueIn(this.Root);
                value = Math.Clamp(value, min, max);
            }
            
            return value;
        }

        internal int RecomputeValue() {
            double @base = this.ExecuteModificationRules(this.BaseValue);
            this.Value = this.Mode switch {
                ModifierMode.ByPriority => (int)this.Modifiers.Values.Aggregate(@base, modify),
                ModifierMode.ByTimeOrder => (int)this.ModifierSequence.Aggregate(@base, modify),
                var _ => throw new ArgumentException("Invalid modifier mode")
            };
            
            return this.Value;
            double modify(double value, Modifier m) => this.ExecuteModificationRules(m.Modify(value));
        }

        internal void AddModifier(Modifier modifier) {
            this.ModifierSequence.AddLast(modifier);
            if (!this.Modifiers.TryAdd(modifier.Type, modifier)) {
                this.Modifiers[modifier.Type] += modifier;
            }
        }

        internal void RemoveModifier(Modifier modifier) {
            LinkedListNode<Modifier> node = this.ModifierSequence.FindLast(modifier);
            if (node is null) {
                Debug.LogError($"Modifier {modifier} not found!");
                return;
            }

            this.ModifierSequence.Remove(node);
            this.Modifiers[modifier.Type] -= modifier;
        }

        /// <summary>
        /// Project the value of this attribute after applying the given modifier.
        /// </summary>
        /// <param name="modifier">The modifier to be applied.</param>
        /// <returns>The projected value of this attribute after applying the given modifier.</returns>
        internal int Project(Modifier modifier) {
            double value = this.BaseValue;
            IEnumerable<Modifier> modifiers = this.Mode switch {
                ModifierMode.ByPriority => this.Modifiers.Values,
                ModifierMode.ByTimeOrder => this.ModifierSequence,
                var _ => throw new ArgumentException("Invalid modifier mode")
            };
            
            foreach (Modifier m in modifiers) {
                double modified = m.Type == modifier.Type ? (m + modifier).Modify(value) : m.Modify(value);
                value = this.ExecuteModificationRules(modified);
            }

            return (int)value;
        }
    }
}
