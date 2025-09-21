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
        
        internal int MaxValue => Math.Min(int.MaxValue, (int)this.ExecuteModificationRules(float.MaxValue));
        internal int MinValue => Math.Max(int.MinValue, (int)this.ExecuteModificationRules(float.MinValue));

        private SortedList<Modifier.Operation, Modifier> Modifiers { get; } =
            new SortedList<Modifier.Operation, Modifier>();
        
        private LinkedList<Modifier> ModifierSequence { get; } = new LinkedList<Modifier>();

        private AttributeData(float value, AttributeSet root, ModifierMode mode) {
            this.BaseValue = value;
            this.Root = root;
            this.Mode = mode;
        }

        internal static AttributeData From(
            AttributeTypeDefinition definition, float initValue, AttributeSet root, ModifierMode mode
        ) {
            AttributeData data = new AttributeData(initValue, root, mode);
            foreach (IAttributeClampRule rule in definition.ModificationRules) {
                data.ModificationRules.Add(rule);
            }

            return data;
        }

        internal void Clamp() {
            this.Value = (int)this.ExecuteModificationRules(this.Value);
        }

        private float ExecuteModificationRules(float value) {
            foreach (IAttributeClampRule rule in this.ModificationRules) {
                value = Math.Clamp(value, rule.MinValueIn(this.Root), rule.MaxValueIn(this.Root));
            }
            
            return value;
        }

        private int RecomputeValue() {
            return this.Mode switch {
                ModifierMode.ByPriority => (int)this.Modifiers.Values.Aggregate(this.BaseValue, modify),
                ModifierMode.ByTimeOrder => (int)this.ModifierSequence.Aggregate(this.BaseValue, modify),
                var _ => throw new ArgumentException("Invalid modifier mode")
            };
            
            float modify(float value, Modifier m) => this.ExecuteModificationRules(m.Modify(value));
        }

        internal void AddModifier(Modifier modifier) {
            this.ModifierSequence.AddLast(modifier);
            if (!this.Modifiers.TryAdd(modifier.Type, modifier)) {
                this.Modifiers[modifier.Type] += modifier;
            }

            // Apply each modifier sequentially and clamp the value to the range after each modification.
            this.Value = this.RecomputeValue();
        }

        internal void RemoveModifier(Modifier modifier) {
            LinkedListNode<Modifier> node = this.ModifierSequence.FindLast(modifier);
            if (node is null) {
                Debug.LogError($"Modifier {modifier} not found!");
                return;
            }

            this.ModifierSequence.Remove(node);
            this.Modifiers[modifier.Type] -= modifier;
            this.Value = this.RecomputeValue();
        }

        /// <summary>
        /// Project the value of this attribute after applying the given modifier.
        /// </summary>
        /// <param name="modifier">The modifier to be applied.</param>
        /// <returns>The projected value of this attribute after applying the given modifier.</returns>
        internal int Project(Modifier modifier) {
            float value = this.BaseValue;
            IEnumerable<Modifier> modifiers = this.Mode switch {
                ModifierMode.ByPriority => this.Modifiers.Values,
                ModifierMode.ByTimeOrder => this.ModifierSequence,
                var _ => throw new ArgumentException("Invalid modifier mode")
            };
            
            foreach (Modifier m in modifiers) {
                float modified = m.Type == modifier.Type ? (m + modifier).Modify(value) : m.Modify(value);
                value = this.ExecuteModificationRules(modified);
            }

            return (int)value;
        }
    }
}
