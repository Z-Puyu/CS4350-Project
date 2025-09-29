using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.ModificationRules;
using GameplayAbilities.Runtime.Modifiers;

namespace GameplayAbilities.Runtime.Attributes {
    internal class AttributeSetNode {
        private AttributeSet Root { get; }
        private List<IAttributeClampRule> ClampRules { get; }
        
        private SortedList<Modifier.Operation, Modifier> Modifiers { get; } =
            new SortedList<Modifier.Operation, Modifier>();
        
        internal IEnumerable<Modifier> CurrentModifiers => this.Modifiers.Values;
        
        private int @base;
        internal int BaseValue { get => this.@base; set => this.@base = (int)this.Clamp(value); }
        internal int Value { get; private set; }
        
        internal int MaxValue => Math.Min(int.MaxValue, (int)this.Clamp(int.MaxValue));
        internal int MinValue => Math.Max(int.MinValue, (int)this.Clamp(int.MinValue));

        private AttributeSetNode(int value, AttributeSet root, List<IAttributeClampRule> clampRules) {
            this.BaseValue = value;
            this.Root = root;
            this.ClampRules = clampRules;
        }

        internal static AttributeSetNode From(AttributeType definition, AttributeSet root) {
            return new AttributeSetNode(0, root, definition.ModificationRules);
        }

        private double Clamp(double value) {
            foreach (IAttributeClampRule rule in this.ClampRules) {
                int min = rule.MinValueIn(this.Root);
                int max = rule.MaxValueIn(this.Root);
                value = Math.Clamp(value, min, max);
            }
            
            return value;
        }
        
        internal int EvaluateWithBase(double baseValue) {
            baseValue = this.Clamp(baseValue);
            return (int)this.Modifiers.Values.Aggregate(baseValue, modify);
            
            double modify(double value, Modifier m) => this.Clamp(m.Modify(value));
        }
        
        internal int RecomputeValue() {
            return this.Value = this.EvaluateWithBase(this.BaseValue);
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
                value = this.Clamp(modified);
            }

            return (int)value;
        }

        internal AttributeSetNode Clone() {
            AttributeSetNode clone = new AttributeSetNode(this.BaseValue, this.Root, this.ClampRules);
            foreach (Modifier modifier in this.Modifiers.Values) {
                clone.AddModifier(modifier);
            }
            
            return clone;
        }
    }
}
