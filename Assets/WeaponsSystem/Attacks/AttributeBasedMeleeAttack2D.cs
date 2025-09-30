using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;
using Weapons.Runtime;
using WeaponsSystem.DamageHandling;
using WeaponsSystem.Projectiles;
using Damage = Weapons.Runtime.Damage;

namespace WeaponsSystem.Attacks {
    public class AttributeBasedMeleeAttack2D : AttributeBasedAttack {
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))] 
        private string RangeAttribute { get; set; }

        [field: SerializeField, PropRange(0, 180)]
        private float SweepHalfAngle { get; set; } = 60f;
        
        public override float Execute(AttackContext context) {
            int range = context.WeaponStats.Get(this.RangeAttribute);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(context.AttackPoint, range, context.AttackableLayers);
            foreach (Collider2D c in colliders) {
                if (context.AttackableTags.Count > 0 && !context.AttackableTags.Any(c.CompareTag)) {
                    continue;
                }

                float angle = Vector2.Angle(context.Direction, c.transform.position - context.AttackPoint);
                if (angle > this.SweepHalfAngle) {
                    continue;
                }

                if (!c.TryGetComponent(out IDamageable damageable) || !this.AllowsDamageOn(c.gameObject)) {
                    continue;
                }
                
                this.PerformDamage(damageable, context.WeaponStats);
            }
            
            return 0;
        }
        
        public override bool AllowsDamageOn(GameObject target) {
            return true;
        }
    }
}
