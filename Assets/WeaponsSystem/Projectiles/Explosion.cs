using System.Collections.Generic;
using Common;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.Projectiles {
    [DisallowMultipleComponent]
    public sealed class Explosion : ProjectileEffect {
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))] 
        private string ExplosionRadiusAttribute { get; set; }
        
        private AdvancedDropdownList<string> AttributeOptions => this.GetAttributeOptions();
        
        public override bool IsEnabledFor(Projectile projectile) {
            return projectile.SpecialEffects.HasFlag(Projectile.OnHitReaction.Explode);
        }

        public override void Execute(Projectile projectile) {
            OnScreenDebugger.Log("Exploded");
        }

        public override IEnumerable<string> GetRequiredAttributes() {
            return new[] { this.ExplosionRadiusAttribute };
        }
    }
}
