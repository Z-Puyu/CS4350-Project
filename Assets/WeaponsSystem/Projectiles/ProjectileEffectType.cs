using UnityEngine;

namespace WeaponsSystem.Projectiles {
    [CreateAssetMenu(fileName = "New Projectile Effect Type", menuName = "Weapons/Projectile Effect Type")]
    public sealed class ProjectileEffectType : ScriptableObject {
        [field: SerializeField] private string Name { get; set; }
    }
}
