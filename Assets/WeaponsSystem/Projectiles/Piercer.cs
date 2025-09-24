using System.Collections.Generic;
using Common;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.Projectiles {
    [DisallowMultipleComponent]
    public class Piercer : ProjectileEffect {
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))] 
        private string PierceCountAttribute { get; set; }
        
        private int RemainingPierce { get; set; }
        
        private AdvancedDropdownList<string> AttributeOptions => this.GetAttributeOptions();

        public override void Execute(Projectile projectile, LayerMask mask, IEnumerable<string> tags) {
            this.RemainingPierce -= 1;
            OnScreenDebugger.Log($"Remaining Pierce Count: {this.RemainingPierce} times");
            if (this.RemainingPierce > 0) {
                projectile.Relaunch();
            }
        }

        public override void FetchAttributes(IAttributeReader source) {
            this.RemainingPierce = source.GetCurrent(this.PierceCountAttribute);
        }
    }
}
