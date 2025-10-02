using WeaponsSystem.Runtime.DamageHandling;

namespace WeaponsSystem.Runtime.Weapons {
    public interface IDamageable {
        public void HandleDamage(Damage damage);
    }
}
