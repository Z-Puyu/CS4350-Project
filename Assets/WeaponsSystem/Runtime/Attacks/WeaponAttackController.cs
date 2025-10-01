using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WeaponsSystem.Runtime.Weapons;

namespace WeaponsSystem.Runtime.Attacks {
    public class WeaponAttackController : WeaponController {
        [field: SerializeReference]
        private List<AttackStrategy> AttackStrategies { get; set; } = new List<AttackStrategy>();
        
        [field: SerializeField] private LayerMask AttackableLayers { get; set; }
        [field: SerializeField] private List<string> IncludeTags { get; set; } = new List<string>();
        [field: SerializeField] private List<string> ExcludeTags { get; set; } = new List<string>();
        
        public override void UpdateOnAttack(ref AttackAction action) {
            int index = this.Weapon.CurrentComboIndex;
            if (index >= this.AttackStrategies.Count) {
#if DEBUG
                Debug.LogWarning($"Attack index {index} out of bounds for weapon {this.Weapon.name}");
#endif
                index = 0;
            }
            
            AttackContext context = this.ContextOf(ref action);
            this.Weapon.AttackDuration = this.AttackStrategies[index].Execute(ref context);
        }

        public override void UpdatePostAttack() { }

        private AttackContext ContextOf(ref AttackAction action) {
            List<string> attackableTags = action.AttackableTags
                                                .Except(this.ExcludeTags)
                                                .Concat(this.IncludeTags)
                                                .Distinct()
                                                .ToList();
            LayerMask mask = action.AttackableLayers & this.AttackableLayers;
            return new AttackContext(
                action.Instigator, this.Weapon, mask, attackableTags, action.AttackPoint, action.Forward, this.Stats,
                this.Weapon.ProjectileMode
            );
        }
    }
}
