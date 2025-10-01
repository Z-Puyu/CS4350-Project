using System.Linq;
using SaintsField;
using UnityEngine;
using WeaponsSystem.Runtime.Weapons;

namespace WeaponsSystem.Runtime.Attacks {
    public class MeleeAttackStrategy2D : AttackStrategy {
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))] 
        private string MeleeRangeAttribute { get; set; }
        
        
        [field: SerializeField, PropRange(0, 180)] 
        private int SweepHalfAngle { get; set; } = 60;
        
        public override float Execute(ref AttackContext context) {
            int range = context.WeaponStats.GetCurrent(this.MeleeRangeAttribute);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(context.AttackPoint, range, context.AttackableLayers);
            foreach (Collider2D c in colliders) {
                if (this.AttackAttributes.Count > 0 && !context.AttackableTags.Any(c.CompareTag)) {
                    continue;
                }
                
                float angle = Vector2.Angle(context.AttackDirection, c.transform.position - context.AttackPoint);
                if (angle > this.SweepHalfAngle) {
                    continue;
                }

                if (!c.TryGetComponent(out IDamageable damageable) ||
                    !this.AllowsDamageOn(c.gameObject, context.Instigator)) {
                    continue;
                }

                damageable.HandleDamage(this.DealDamage(context));
            }
            
            return 0f;
        }
    }
}
