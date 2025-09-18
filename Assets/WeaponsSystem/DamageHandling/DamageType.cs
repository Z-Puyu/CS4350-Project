using System;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.DamageHandling {
    [Serializable]
    public struct DamageType {
        [field: SerializeField] public AttributeTypeDefinition DamageAttribute { get; private set; }
        
        [field: SerializeField, TableColumn("Defended by")] 
        public AttributeTypeDefinition DefenceAttribute { get; private set; }
        
        [field: SerializeField] public bool IsPercentageDefence { get; private set; }
        [field: SerializeField, MinValue(0)] public int DefenceCoefficient { get; private set; }
    }
}
