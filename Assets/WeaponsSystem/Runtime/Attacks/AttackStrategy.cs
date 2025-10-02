using System;
using System.Collections.Generic;
using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;
using WeaponsSystem.Runtime.DamageHandling;

namespace WeaponsSystem.Runtime.Attacks {
    [Serializable]
    public abstract class AttackStrategy {
        [field: SerializeField, Table]
        protected List<AttackAttribute> AttackAttributes { get; private set; } = new List<AttackAttribute>();
        
        protected AdvancedDropdownList<string> AttributeOptions => this.GetAttributeOptions();
        
        public abstract float Execute(ref AttackContext context, HashSet<IAbility> attachedAbilities);
        
        public virtual bool AllowsDamageOn(GameObject target, GameObject instigator) {
            if (!target || !instigator) {
                return false;
            }

            return target != instigator && !target.transform.IsChildOf(instigator.transform) &&
                   !instigator.transform.IsChildOf(target.transform);
        }

        protected Damage DealDamage(AttackContext context) {
            Damage damage = new Damage(context.Instigator);
            foreach (AttackAttribute attribute in this.AttackAttributes) {
                int value = Mathf.RoundToInt(context.WeaponStats.GetCurrent(attribute.Id) * attribute.Coefficient);
                damage.Set(attribute.Id, value);
            }

            return damage;
        }
    }
}
