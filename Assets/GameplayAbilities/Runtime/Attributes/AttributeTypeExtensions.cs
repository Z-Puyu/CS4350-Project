using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using log4net.Filter;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.Attributes {
    public static class AttributeTypeExtensions {
        private static AdvancedDropdownList<string> GetAttributeOptions(this AttributeType attribute) {
            if (!attribute.IsCategory) {
                return new AdvancedDropdownList<string>(displayNameOf(attribute.Id), attribute.Id);
            }

            return new AdvancedDropdownList<string>(
                displayNameOf(attribute.Id), 
                attribute.SubTypes.Select(subType => subType.GetAttributeOptions())
                         .OrderBy(section => section.displayName)
            );
            
            string displayNameOf(string id) =>
                    id.Contains('.') ? convertToWords(id[(id.LastIndexOf('.') + 1)..]) : convertToWords(id);

            string convertToWords(string input) {
                if (string.IsNullOrEmpty(input)) {
                    return input;
                }

                const string pattern = "(?<=[a-z])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])";
                string[] words = Regex.Split(input, pattern);
                return string.Join(' ', words);
            }
        }
        
        public static AdvancedDropdownList<string> GetAttributeOptions(this object obj) {
            IEnumerable<AdvancedDropdownList<string>> sections =
                    AttributeType.GetAll()
                                 .Where(attribute => attribute.IsRoot)
                                 .Select(attribute => attribute.GetAttributeOptions())
                                 .OrderBy(section => section.displayName);
            return new AdvancedDropdownList<string>("Attributes", sections);
        }
        
        public static DropdownList<string> GetAllAttributes(this object obj) {
            IEnumerable<string> options = AttributeType.GetAll().Select(attribute => attribute.Id);
            return new DropdownList<string>(options.Select(id => (id, id)).OrderBy(pair => pair.Item2));
        }
    }
}
