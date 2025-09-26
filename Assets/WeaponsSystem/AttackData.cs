using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem {
    [Serializable]
    public class AttackData {
        [field: SerializeField, Table]
        private List<ModifierData> Modifiers { get; set; } = new List<ModifierData>();
        
        public bool IsEmpty => this.Modifiers.Count == 0;
        
        public IEnumerable<Modifier> GenerateModifiers(IAttributeReader currentAttributes) {
            GameplayEffectExecutionArgs args = GameplayEffectExecutionArgs.From(currentAttributes, null).Build();
            return this.Modifiers.Select(m => m.CreateModifier(currentAttributes));
        }
    }
}
