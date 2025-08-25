using System;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GameplayAbilities.Runtime.GameplayEffects {
    [Serializable]
    public class EffectCommitmentCost {
        private enum AffordabilityPolicy {
            [RichLabel("Have Strictly More")] HaveStrictlyMore,
            [RichLabel("Have Enough")] HaveEnough,
            [RichLabel("Not at Minimum")] HaveAny,
            [RichLabel("Will not Hit Limit")] WillNotHitLimit,
            [RichLabel("Will not Overflow")] WillNotOverflow,
            [RichLabel("Not at Maximum")] HaveRoomForMore 
        }
        
        [field: SerializeField, ValidateInput(nameof(this.IsValidAttribute))]
        [field: LayoutStart("Numerical", ELayout.Horizontal)]
        private AttributeTypeDefinition Attribute { get; set; }
        
        [field: SerializeField, MinValue(0)] 
        private int Value { get; set; }
        
        [field: SerializeField, OnValueChanged(nameof(this.ValidateAffordability)), LayoutEnd]
        private bool WillAddInsteadOfUse { get; set; }
        
        [field: SerializeField, RichLabel("Affordable If")]
        [field: Dropdown(nameof(this.GetAffordabilityOptions))]
        private AffordabilityPolicy Affordability { get; set; }

        internal bool IsAffordable(IAttributeReader source) {
            string attribute = this.Attribute.Id;
            if (this.WillAddInsteadOfUse) {
                int roomUntilLimit = source.GetMax(attribute) - source.GetCurrent(attribute);
                return roomUntilLimit > 0 && this.Affordability switch {
                    AffordabilityPolicy.WillNotHitLimit => roomUntilLimit > this.Value,
                    AffordabilityPolicy.WillNotOverflow => roomUntilLimit >= this.Value,
                    AffordabilityPolicy.HaveRoomForMore => true,
                    var _ => throw new ArgumentOutOfRangeException()
                };
            }

            int distFromMin = source.GetCurrent(attribute) - source.GetMin(attribute);
            return distFromMin > 0 && this.Affordability switch {
                AffordabilityPolicy.HaveStrictlyMore => distFromMin > this.Value,
                AffordabilityPolicy.HaveEnough => distFromMin >= this.Value,
                AffordabilityPolicy.HaveAny => true,
                var _ => throw new ArgumentOutOfRangeException()
            };
        }

        /// <summary>
        /// Commits the cost to the consumer's attribute set.
        /// </summary>
        /// <param name="consumer">The actor that will consume the cost to commit an effect.</param>
        internal void Commit(AttributeSet consumer) {
            int value = this.WillAddInsteadOfUse ? this.Value : -this.Value;
            consumer.AddModifier(new Modifier(value, Modifier.Operation.Offset, this.Attribute.Id));
        }

        private DropdownList<AffordabilityPolicy> GetAffordabilityOptions() {
            return this.WillAddInsteadOfUse
                    ? new DropdownList<AffordabilityPolicy> {
                        { "Will not Hit Limit", AffordabilityPolicy.WillNotHitLimit },
                        { "Will not Overflow", AffordabilityPolicy.WillNotOverflow },
                        { "Not at Maximum", AffordabilityPolicy.HaveRoomForMore }
                    }
                    : new DropdownList<AffordabilityPolicy> {
                        { "Have Strictly More", AffordabilityPolicy.HaveStrictlyMore },
                        { "Have Enough", AffordabilityPolicy.HaveEnough },
                        { "Not at Minimum", AffordabilityPolicy.HaveAny },
                    };
        }

        private void ValidateAffordability() {
            this.Affordability = this.WillAddInsteadOfUse switch {
                true when this.Affordability is AffordabilityPolicy.HaveStrictlyMore 
                                             or AffordabilityPolicy.HaveEnough
                                             or AffordabilityPolicy.HaveAny =>
                        AffordabilityPolicy.WillNotHitLimit,
                false when this.Affordability is AffordabilityPolicy.WillNotHitLimit
                                              or AffordabilityPolicy.WillNotOverflow
                                              or AffordabilityPolicy.HaveRoomForMore =>
                        AffordabilityPolicy.HaveStrictlyMore,
                var _ => this.Affordability
            };
        }

        private string IsValidAttribute() {
            return this.Attribute && !this.Attribute.IsCategory
                    ? null
                    : "Attribute type must be a leaf type without subtypes!";
        }
    }
}
