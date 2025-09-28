using System.Collections.Generic;
using Common;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.Projectiles {
    [CreateAssetMenu(fileName = "Piercer", menuName = "Projectiles/Effects/Piercing")]
    public class Piercer : ProjectileEffect {
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))] 
        private string PierceCountAttribute { get; set; }

        public override void Execute(
            Projectile projectile, LayerMask mask, IEnumerable<string> tags, ProjectileEffectController controller
        ) {
            controller.UpdateAttribute(this.PierceCountAttribute, -1);
#if DEBUG
            OnScreenDebugger.Log($"Remaining Pierce Count: {controller.Get(this.PierceCountAttribute)} times");
#endif
            
            if (controller.Get(this.PierceCountAttribute) > 0) {
                projectile.Relaunch();
            }
        }

        protected override ICollection<string> RequiredAttributes => new[] { this.PierceCountAttribute };
    }
}
