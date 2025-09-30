using System.Collections.Generic;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using UnityEngine;
using Weapons.Runtime;
using WeaponsSystem.Attacks;
using WeaponsSystem.Projectiles;

namespace WeaponsSystem.WeaponComponents {
    [CreateAssetMenu(fileName = "New Component", menuName = "Weapons/Components/Weapon Component")]
    public class AttributeBasedWeaponComponent : WeaponComponent {
        [field: SerializeField] public string Name { get; private set; }

        [field: SerializeField, Table]
        public List<ComponentModifier> WeaponModifiers { get; private set; } = new List<ComponentModifier>();

        [field: SerializeField, Table] 
        public List<ComponentAttackModifier> AttackData { get; set; } = new List<ComponentAttackModifier>();

        [field: SerializeField]
        public List<ProjectileEffect> ProjectileEffects { get; private set; } = new List<ProjectileEffect>();

        private string AttackDataLabels(AttributeBasedAttack obj, int index) =>
                obj is null ? $"Combo index {index}: no modifiers" : $"Combo index {index}";

        public override void Modify(Weapon weapon, WeaponStats stats) {
            foreach (ComponentModifier modifier in this.WeaponModifiers) {
                stats.Modify(modifier.Target, modifier.Magnitude, modifier.Type);
            }
        }

        public override void UndoEffects(Weapon weapon, WeaponStats stats) {
            foreach (ComponentModifier modifier in this.WeaponModifiers) {
                stats.Modify(modifier.Target, -modifier.Magnitude, modifier.Type);
            }
        }
    }
}
