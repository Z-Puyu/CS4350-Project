using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;

namespace GameplayAbilities.Runtime.Projectiles {
    public interface IProjectileHandler {
        public AttributeSet AttributeSet { get; }
        
        public void Handle(Projectile2D projectile);
    }
}
