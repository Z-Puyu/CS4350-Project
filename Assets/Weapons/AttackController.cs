using System.Collections.Generic;
using GameplayEffects.Runtime;
using SaintsField;
using UnityEngine;

namespace Weapons {
    public sealed class AttackController : IAttackController {
        [field: SerializeReference, SaintsDictionary("Combo Index", "Attack Effects")] 
        private SaintsDictionary<int, Attack> ComboAttacks { get; } = new SaintsDictionary<int, Attack>();
        
        public void Configure(WeaponStats stats, int comboIndex = 0) {
            if (this.ComboAttacks.TryGetValue(comboIndex, out Attack attack)) {
                attack.Apply(stats);
            }
        }
    }
}
