using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;

namespace GameplayAbilities.Runtime.Projectiles {
    public interface IProjectileEffectHandler {
        public void Handle(IEffect<IDataReader<string, int>, AttributeSet> effect, IDataReader<string, int> sender);
    }
}
