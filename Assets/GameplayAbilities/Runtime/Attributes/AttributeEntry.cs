using System;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.Attributes {
    [Serializable]
    public struct AttributeEntry {
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions)), ReadOnly] 
        public string Id { get; private set; }
        
        [field: SerializeField] public int Value { get; private set; }

        private AdvancedDropdownList<string> AttributeOptions => this.GetAttributeOptions();
        
        public AttributeEntry(string id, int value) {
            this.Id = id;
            this.Value = value;
        }
    }
}
