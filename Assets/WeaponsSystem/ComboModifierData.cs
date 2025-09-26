using System;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Modifiers;
using UnityEngine;

namespace WeaponsSystem {
    [Serializable]
    public class ComponentModifierData {
        [field: SerializeField] public AttackModifierData ModifierData { get; private set; }
        [field: SerializeField] public int ComboIndex { get; private set; }
        [field: SerializeField] public bool ModifySingleAttack { get; private set; }
        
        public bool ModifyCombo() {
            return this.ModifySingleAttack;
        }
    }
}
