using System;
using UnityEngine;

namespace WeaponsSystem {
    [Serializable]
    public struct AttackData {
        [field: SerializeField] private float damageModifier;
        [field: SerializeField] private float knockbackModifier;
        
        public float Damage => this.damageModifier;
    }
}
