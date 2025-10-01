using DataStructuresForUnity.Runtime.Utilities;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.Weapons {
    [DisallowMultipleComponent]
    public class Weapon : MonoBehaviour {
        [field: SerializeField, Required] private AttributeSet Stats { get; set; }
        
        public int CurrentComboIndex { get; private set; }
        
        private Timer ComboResetTimer { get; set; }

        public void NextCombo(int comboLength) {
            this.CurrentComboIndex += 1;
            this.CurrentComboIndex %= comboLength;
        }

        public void ResetComboAfter(float time) {
            this.ComboResetTimer = new Timer(time);
            this.ComboResetTimer.OnTimerFinished += this.ResetCombo;
            this.ComboResetTimer.Start();
        }

        private void ResetCombo() {
            this.CurrentComboIndex = 0;
        }
    }
}
