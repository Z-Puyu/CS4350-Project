using DataStructuresForUnity.Runtime.Utilities;
using SaintsField;
using UnityEngine;

namespace Weapons.Runtime {
    public class WeaponComboController : WeaponController {
        [field: SerializeField, MinValue(1)] private int MaxCombo { get; set; }
        [field: SerializeField, MinValue(1f)] public float ComboResetTime { get; private set; }
        private int currentComboIndex;
        
        public virtual int ComboLength => this.MaxCombo;

        public int CurrentComboIndex {
            get => this.currentComboIndex;
            set => this.currentComboIndex = value % this.ComboLength;
        }
        
        public override float UpdateOnAttack(AttackAction action) {
            this.CurrentComboIndex += 1;
            return 0;
        }
    }
}
