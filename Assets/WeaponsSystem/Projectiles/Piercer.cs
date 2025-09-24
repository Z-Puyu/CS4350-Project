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

        public override bool IsEnabledFor(Projectile projectile) {
            return projectile.MotionType == Projectile.Motion.Pierce;
        }

        public override void TurnOn(Projectile projectile) {
            base.TurnOn(projectile);
            this.RemainingPierce = projectile.GetAttribute(this.PierceCountAttribute);
        }

        public override void Execute(Projectile projectile) {
            this.RemainingPierce -= 1;
            OnScreenDebugger.Log($"Remaining Pierce Count: {this.RemainingPierce} times");
            if (this.RemainingPierce <= 0) {
                projectile.MarkForDestruction();
            }
        }
        
        public override IEnumerable<string> GetRequiredAttributes() {
            return new[] { this.PierceCountAttribute };
        }
    }
}
