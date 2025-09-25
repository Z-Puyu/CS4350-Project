using System;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem {
    [Serializable]
    public struct AttackModifierData {
        [field: SerializeField, Dropdown(nameof(this.AttributeOptions))] 
        public string Target { get; private set; }
        
        [field: SerializeField] public Modifier.Operation Type { get; private set; }
        [field: SerializeField] public int Magnitude { get; private set; }
        
        private DropdownList<string> AttributeOptions => this.GetAllAttributes();
    }
}
