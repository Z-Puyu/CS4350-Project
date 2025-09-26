using System.Collections.Generic;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using UnityEngine;
using WeaponsSystem.Projectiles;

namespace WeaponsSystem.WeaponComponent {
    [CreateAssetMenu(fileName = "ComponentData", menuName = "Weapons/Components/Component Data")]
    public class WeaponComponentData : ScriptableObject {
        [field: SerializeField] public string Name { get; private set; }

        [field: SerializeField, Table]
        public List<ModifierData> WeaponModifiers { get; private set; } = new List<ModifierData>();

        [field: SerializeField, RichLabel(nameof(this.AttackDataLabels), true)] 
        public List<AttackData> AttackData { get; set; } = new List<AttackData>(); 
        
        [field: SerializeField] private List<ProjectileEffect> ProjectileEffects { get; set; } = new List<ProjectileEffect>();

        private string AttackDataLabels(AttackData obj, int index) =>
                obj is null || obj.IsEmpty ? $"Combo index {index}: no modifiers" : $"Combo index {index}";
    }
}
