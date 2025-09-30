using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Modifiers;
using GameplayEffects.Runtime;
using SaintsField;
using UnityEngine;
using Weapons.Runtime;
using WeaponsSystem.Projectiles;
using ProjectileEffect = WeaponsSystem.Projectiles.ProjectileEffect;

namespace WeaponsSystem.Attacks {
    [Serializable]
    public abstract class AttributeBasedAttack : AttackStrategy {
        [field: SerializeField, Table]
        private List<AttackAttribute> AttackAttributes { get; set; } = new List<AttackAttribute>();
        
        protected AdvancedDropdownList<string> AttributeOptions => this.GetAttributeOptions();

        protected void PerformDamage(IDamageable target, WeaponStats weaponStats) {
            Damage damage = new Damage();
            foreach (AttackAttribute attribute in this.AttackAttributes) {
                int magnitude = weaponStats.Get(attribute.Id);
                damage.Set(attribute.Id, Mathf.RoundToInt(magnitude * attribute.Coefficient));
            }
                
            target.HandleDamage(damage);
        }
    }
}
