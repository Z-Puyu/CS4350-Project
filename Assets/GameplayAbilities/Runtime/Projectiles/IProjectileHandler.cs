using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;

namespace GameplayAbilities.Runtime.Projectiles {
    public interface IProjectileHandler {
        public void Handle(Projectile projectile);
    }
}
