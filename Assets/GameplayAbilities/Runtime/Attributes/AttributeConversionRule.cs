using System;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.Attributes {
    [Serializable]
    internal struct AttributeConversionRule {
        [field: SerializeField, RichLabel("Every ")] 
        private AttributeType From { get; set; }
        
        [field: SerializeField, RichLabel("Is Equivalent to ")]
        private float ConversionRate { get; set; }
        
        [field: SerializeField, ValidateInput(nameof(this.IsValidDestinationAttribute))] 
        private AttributeType To { get; set; }

        internal bool TryConvert(float value, string from, string to, out float convertedValue) {
            if (this.From.Includes(from) && this.To.Id == to) {
                convertedValue = this.ConversionRate * value;
                return true;
            }
            
            convertedValue = value;
            return false;
        }

        private string IsValidDestinationAttribute(AttributeType attribute) {
            return attribute.IsCategory ? "Destination attribute must be a leaf attribute" : null;
        }
    }
}
