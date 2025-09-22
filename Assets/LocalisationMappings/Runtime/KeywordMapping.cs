using System;
using SaintsField;
using DataStructuresForUnity.Runtime.GeneralUtils;
using UnityEngine;

namespace LocalisationMappings.Runtime {
    [Serializable]
    internal sealed class KeywordMapping {
        private DropdownList<string> PropertiesAndMethods { get; set; }
        [field: SerializeField, ReadOnly] public string Keyword { get; set; }
        [field: SerializeField, Dropdown(nameof(this.PropertiesAndMethods))] 
        public string Value { get; set; }
        
        private KeywordMapping(string keyword, DropdownList<string> propertiesAndMethods) {
            this.Keyword = keyword;
            this.PropertiesAndMethods = propertiesAndMethods;
        }

        public static KeywordMapping Of(string keyword, Type type) {
            DropdownList<string> propertiesAndMethods = new DropdownList<string>();
            foreach (string property in type.GetPropertyGetterNames()) {
                propertiesAndMethods.Add(property, property);
            }

            foreach (string method in type.GetProducerMethodNames()) {
                propertiesAndMethods.Add(method, method);
            }
            
            foreach (string field in type.GetFieldNames()) {
                propertiesAndMethods.Add(field, field);
            }

            return new KeywordMapping(keyword, propertiesAndMethods);
        }
    }
}
