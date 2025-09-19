using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem {
    [Serializable]
    public class AttackData {
        [field: SerializeField, Table]
        private List<AttackModifierData> DefaultModifiers { get; set; } = new List<AttackModifierData>();
        
        private Dictionary<(string, Modifier.Operation), Modifier> Modifiers { get; set; } =
            new Dictionary<(string, Modifier.Operation), Modifier>();

        public IEnumerable<Modifier> WeaponModifiers => this.Modifiers.Values;

        public void Initialise() {
            foreach (AttackModifierData data in this.DefaultModifiers) {
                this.AddModifier(new Modifier(data.Magnitude, data.Type, data.Target.Id));
            }
        }
        
        public void AddModifier(Modifier modifier) {
            if (!this.Modifiers.TryAdd((modifier.Target, modifier.Type), modifier)) {
                this.Modifiers[(modifier.Target, modifier.Type)] += modifier;
            }
        }
    }
}
