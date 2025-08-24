using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAttributes.Runtime.ModificationRules;
using GameplayAttributes.Runtime.Modifiers;

namespace GameplayAttributes.Runtime {
    internal class AttributeData {
        private AttributeSet Root { get; }
        private List<IAttributeModificationRule> ModificationRules { get; }
        private float BaseValue { get; }
        internal int Value { get; private set; }
        
        internal int MaxValue => Math.Min(int.MaxValue, (int)this.ExecuteModificationRules(float.MaxValue));
        internal int MinValue => Math.Max(int.MinValue, (int)this.ExecuteModificationRules(float.MinValue));

        private SortedList<Modifier.Operation, Modifier> Modifiers { get; } =
            new SortedList<Modifier.Operation, Modifier>();

        private AttributeData(List<IAttributeModificationRule> modificationRules, float value, AttributeSet root) {
            this.ModificationRules = modificationRules;
            this.BaseValue = value;
            this.Root = root;
        }

        internal static AttributeData From(AttributeTypeDefinition definition, float initValue, AttributeSet root) {
            List<IAttributeModificationRule> modificationRules = definition.ModificationRules.ToList();
            return new AttributeData(modificationRules, initValue, root);
        }
        
        internal void Clamp() {
            this.Value = (int)this.ExecuteModificationRules(this.Value);
        }

        private float ExecuteModificationRules(float value) {
            foreach (IAttributeModificationRule rule in this.ModificationRules) {
                value = rule.Apply(value, this.Root);
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
    }
}
