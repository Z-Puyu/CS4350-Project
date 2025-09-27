using DataStructuresForUnity.Runtime.GeneralUtils;

namespace GameplayAbilities.Runtime.Abilities {
    public abstract class SpawnableAbilityObject : PoolableObject, ISpawnable<AbilityInfo> {
        public abstract void Activate(AbilityInfo info);
        public abstract void Destroy();
    }
}
