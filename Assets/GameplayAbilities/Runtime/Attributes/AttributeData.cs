using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.ModificationRules;
using GameplayAbilities.Runtime.Modifiers;

namespace GameplayAbilities.Runtime.Attributes {
    internal class AttributeData {
        private AttributeSet Root { get; }
        private List<IAttributeClampRule> ModificationRules { get; }
        private float BaseValue { get; }
        internal int Value { get; private set; }
        
        internal int MaxValue => Math.Min(int.MaxValue, (int)this.ExecuteModificationRules(float.MaxValue));
        internal int MinValue => Math.Max(int.MinValue, (int)this.ExecuteModificationRules(float.MinValue));

        private SortedList<Modifier.Operation, Modifier> Modifiers { get; } =
            new SortedList<Modifier.Operation, Modifier>();

        private AttributeData(
            List<IAttributeClampRule> modificationRules, float value, AttributeSet root
        ) {
            this.ModificationRules = modificationRules;
            this.BaseValue = value;
            this.Root = root;
        }

        internal static AttributeData From(AttributeTypeDefinition definition, float initValue, AttributeSet root) {
            List<IAttributeClampRule> rules = definition.ModificationRules.ToList();
            return new AttributeData(rules, initValue, root);
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

        internal void AddModifier(Modifier modifier) {
            if (this.Modifiers.TryGetValue(modifier.Type, out Modifier curr)) {
                this.Modifiers[modifier.Type] = curr + modifier;
            } else {
                this.Modifiers.Add(modifier.Type, modifier);
            }
            
            // Apply each modifier sequentially and clamp the value to the range after each modification.
            this.Value = (int)this.Modifiers.Values.Aggregate(this.BaseValue, modify);
            return;

            float modify(float value, Modifier m) => this.ExecuteModificationRules(m.Modify(value));
        }

        /// <summary>
        /// Project the value of this attribute after applying the given modifier.
        /// </summary>
        /// <param name="modifier">The modifier to be applied.</param>
        /// <returns>The projected value of this attribute after applying the given modifier.</returns>
        internal int Project(Modifier modifier) {
            float value = this.BaseValue;
            foreach (KeyValuePair<Modifier.Operation, Modifier> pair in this.Modifiers) {
                float modified = pair.Key == modifier.Type
                        ? (pair.Value + modifier).Modify(value)
                        : pair.Value.Modify(value);
                value = this.ExecuteModificationRules(modified);
            }

            return (int)value;
        }
    }
}
