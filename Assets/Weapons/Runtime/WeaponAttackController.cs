using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Weapons.Runtime {
    public class WeaponAttackController : WeaponController {
        [field: SerializeReference]
        private List<AttackStrategy> AttackStrategies { get; set; } = new List<AttackStrategy>();
        
        [field: SerializeField] private LayerMask AttackableLayers { get; set; }
        [field: SerializeField] private List<string> IncludeTags { get; set; } = new List<string>();
        [field: SerializeField] private List<string> ExcludeTags { get; set; } = new List<string>();
        
        public override float UpdateOnAttack(AttackAction action) {
            int index = action.ComboIndex;
            if (action.ComboIndex >= this.AttackStrategies.Count) {
#if DEBUG
                Debug.LogWarning($"Attack index {index} out of bounds for weapon {this.Weapon.name}");
#endif
                index = 0;
            }
            
            return this.AttackStrategies[index].Execute(this.ContextOf(action));
        }

        protected virtual AttackContext ContextOf(AttackAction action) {
            List<string> attackableTags = action.AttackableTags
                                                .Except(this.ExcludeTags)
                                                .Concat(this.IncludeTags)
                                                .Distinct()
                                                .ToList();
            LayerMask mask = action.AttackableLayers & this.AttackableLayers;
            return new AttackContext(
                this.Weapon, mask, attackableTags, action.AttackPoint, action.Direction, this.Stats
            );
        }
    }
}
