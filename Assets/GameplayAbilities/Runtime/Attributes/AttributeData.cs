using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GameplayAbilities.Runtime.ModificationRules;
using GameplayAbilities.Runtime.Modifiers;
using UnityEngine;

namespace GameplayAbilities.Runtime.Attributes {
    internal class AttributeData {
        private AttributeSet Root { get; }
        private List<IAttributeClampRule> ModificationRules { get; } = new List<IAttributeClampRule>();
        private double BaseValue { get; }
        internal int Value { get; private set; }
        
        internal int MaxValue => Math.Min(int.MaxValue, (int)this.ExecuteModificationRules(int.MaxValue));
        internal int MinValue => Math.Max(int.MinValue, (int)this.ExecuteModificationRules(int.MinValue));

        private SortedList<Modifier.Operation, Modifier> Modifiers { get; } =
            new SortedList<Modifier.Operation, Modifier>();

        private AttributeData(double value, AttributeSet root) {
            this.BaseValue = value;
            this.Root = root;
        }

        internal static AttributeData From(AttributeType definition, double initValue, AttributeSet root) {
            AttributeData data = new AttributeData(initValue, root);
            foreach (IAttributeClampRule rule in definition.ModificationRules) {
                data.ModificationRules.Add(rule);
            }
            
            data.Value = data.Query(initValue);
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
        
        internal int Query(double @base) {
            @base = this.ExecuteModificationRules(@base);
            return (int)this.Modifiers.Values.Aggregate(@base, modify);
            
            double modify(double value, Modifier m) => this.ExecuteModificationRules(m.Modify(value));
        }
        
        internal int RecomputeValue() {
            return this.Value = this.Query(this.BaseValue);
        }

        internal void AddModifier(Modifier modifier) {
            if (!this.Modifiers.TryAdd(modifier.Type, modifier)) {
                this.Modifiers[modifier.Type] += modifier;
            }
        }

        internal void RemoveModifier(Modifier modifier) {
            this.Modifiers[modifier.Type] -= modifier;
        }

        /// <summary>
        /// Project the value of this attribute after applying the given modifier.
        /// </summary>
        /// <param name="modifier">The modifier to be applied.</param>
        /// <returns>The projected value of this attribute after applying the given modifier.</returns>
        internal int Project(Modifier modifier) {
            double value = this.BaseValue;
            foreach (Modifier m in this.Modifiers.Values) {
                double modified = m.Type == modifier.Type ? (m + modifier).Modify(value) : m.Modify(value);
                value = this.ExecuteModificationRules(modified);
            }

            return (int)value;
        }
    }
}
