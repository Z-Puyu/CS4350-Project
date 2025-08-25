using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.Attributes {
    [CreateAssetMenu(fileName = "Attribute Conversion Table", menuName = "Gameplay Abilities/Attribute Conversion Table")]
    public class AttributeConversionTable : ScriptableObject {
        [field: SerializeField, Table] 
        private List<AttributeConversionRule> ConversionRules { get; set; } = new List<AttributeConversionRule>();
        
        public bool TryConvert(float value, string from, out string convertedAttribute, out float convertedValue) {
            foreach (AttributeConversionRule rule in this.ConversionRules) {
                if (rule.TryConvert(value, from, out convertedAttribute, out convertedValue)) {
                    return true;
                }
            }
            
            convertedAttribute = from;
            convertedValue = value;
            return false;
        }
        
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
