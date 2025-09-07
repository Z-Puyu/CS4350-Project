using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.Attributes {
    [CreateAssetMenu(fileName = "Attribute Conversion Table", menuName = "Gameplay Abilities/Attribute Conversion Table")]
    public class AttributeConversionTable : ScriptableObject {
        [field: SerializeField, Table] 
        private List<AttributeConversionRule> ConversionRules { get; set; } = new List<AttributeConversionRule>();
        
        /// <summary>
        /// Try to convert the given value from the given attribute to the given attribute.
        /// </summary>
        /// <param name="value">The value of the attribute to convert.</param>
        /// <param name="from">The original attribute.</param>
        /// <param name="to">The attribute to convert to.</param>
        /// <param name="convertedValue">The converted value of the attribute.</param>
        /// <returns><c>true</c> if the conversion is successful, <c>false</c> otherwise.</returns>
        public bool TryConvert(float value, string from, string to, out float convertedValue) {
            foreach (AttributeConversionRule rule in this.ConversionRules) {
                if (rule.TryConvert(value, from, to, out convertedValue)) {
                    return true;
                }
            }
            
            convertedValue = value;
            return false;
        }
    }
}
