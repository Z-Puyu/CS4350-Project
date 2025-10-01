using System.Collections.Generic;
using GameplayAbilities.Runtime.Modifiers;

namespace WeaponsSystem.Weapons {
    public class WeaponModifierController : WeaponController {
        private int ComboIndex { get; set; }

        private Dictionary<int, List<Modifier>> Modifiers { get; set; } =
            new Dictionary<int, List<Modifier>>();
        
        public override void UpdateOnAttack() {
            this.ComboIndex = this.Weapon.CurrentComboIndex;
            if (!this.Modifiers.TryGetValue(this.ComboIndex, out SortedList<Modifier.Operation, Modifier> list)) {
                foreach (Modifier modifier in list.Values) {
                    this.
                }
            }
        }
        
        public override void UpdatePostAttack() {
            
        }
    }
}
