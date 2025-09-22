using System;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.DamageHandling {
    [Serializable]
    public struct DamageType {
        [field: SerializeField, Dropdown(nameof(this.GetAttributeOptions))] 
        public string DamageAttribute { get; private set; }
        
        [field: SerializeField, TableColumn("Defended by"), Dropdown(nameof(this.GetAttributeOptions))] 
        public string DefenceAttribute { get; private set; }
        
        [field: SerializeField] public bool IsPercentageDefence { get; private set; }
        [field: SerializeField, MinValue(0)] public int DefenceCoefficient { get; private set; }

        private DropdownList<string> GetAttributeOptions() {
            DropdownList<string> list = new DropdownList<string>();
            foreach (AttributeType resource in AttributeType.GetAllLeaves()) {
                list.Add(resource.Id, resource.Id);
            }

            return list;
        }
    }
}
