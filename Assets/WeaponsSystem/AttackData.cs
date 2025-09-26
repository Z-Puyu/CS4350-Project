using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using UnityEngine;
using WeaponsSystem.Projectiles;

namespace WeaponsSystem {
    [Serializable]
    public class AttackData {
        [field: SerializeField] public ProjectileSpawner.Mode Mode { get; private set; }
        
        [field: SerializeField, Table]
        private List<ModifierData> Modifiers { get; set; } = new List<ModifierData>();
        
        public bool IsEmpty => this.Modifiers.Count == 0;
        
        public IEnumerable<Modifier> GenerateModifiers(IAttributeReader currentAttributes) {
            return this.Modifiers.Select(m => m.CreateModifier(currentAttributes));
        }
    }
}
