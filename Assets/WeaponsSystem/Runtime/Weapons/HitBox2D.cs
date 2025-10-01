using SaintsField;
using UnityEngine;
using UnityEngine.Events;
using WeaponsSystem.Runtime.DamageHandling;

namespace WeaponsSystem.Runtime.Weapons {
    [DisallowMultipleComponent]
    public sealed class HitBox2D : MonoBehaviour, IDamageable {
        [field: SerializeField] private UnityEvent<Damage> OnHitEvent { get; set; } = new UnityEvent<Damage>();
        
        [field: SerializeField, MinValue(-100)] 
        private int DamageMultiplier { get; set; }

        public void HandleDamage(Damage damage) {
            damage.Multiplier += this.DamageMultiplier;
            this.OnHitEvent.Invoke(damage);
        }
    }
}
