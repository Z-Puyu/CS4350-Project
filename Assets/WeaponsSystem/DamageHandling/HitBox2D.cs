using GameplayAbilities.Runtime.GameplayEffects;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace WeaponsSystem.DamageHandling {
    [DisallowMultipleComponent]
    public sealed class HitBox2D : MonoBehaviour, IDamageable {
        [field: SerializeField, Required] private Collider2D Collider { get; set; }
        [field: SerializeField] private UnityEvent<Damage> OnHitEvent { get; set; } = new UnityEvent<Damage>();
        
        public event UnityAction<Damage> OnHit; 
        
        public void HandleDamage(Damage damage) {
            this.OnHit?.Invoke(damage);
            this.OnHitEvent.Invoke(damage);
        }
    }
}
