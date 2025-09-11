using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using SaintsField;
using UnityEngine;

namespace LocalisationMappings.Runtime {
    [CreateAssetMenu(fileName = "New Localisation Text Formatter", 
        menuName = "Localisation Mappings/Localisation Text Formatter", order = 0)]
    public sealed class LocalisationTextFormatter : ScriptableObject {
        private static readonly Regex KeywordRegex = new Regex("@([A-Za-z][A-Za-z0-9]*(?:-[A-Za-z][A-Za-z0-9]*)*)"); 
        
        [field: SerializeField, TypeReference(EType.AllAssembly), OnValueChanged(nameof(this.OnTargetTypeChanged))] 
        private Type LocalisedObjectType { get; set; }
        [field: SerializeField, TextArea] private string Text { get; set; }
        [field: SerializeField, Table(hideAddButton: true, hideRemoveButton: true)]
        private List<KeywordMapping> Keywords { get; set; }
        private void OnTargetTypeChanged() {
            foreach (Match match in LocalisationTextFormatter.KeywordRegex.Matches(this.Text)) {
                this.Keywords.Add(KeywordMapping.Of(match.Value, this.LocalisedObjectType));
            }
        }

        public string Format(object @object) {
            if (this.LocalisedObjectType.IsInstanceOfType(@object)) {
                return this.Keywords
                           .Aggregate(this.Text, (text, mapping) => text.Replace(mapping.Keyword, mapping.Value));
            }

            Debug.LogError($"{@object} is not an instance of {this.LocalisedObjectType}", this);
            return string.Empty;
        }
    }
}
