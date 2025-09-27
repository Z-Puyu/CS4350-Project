using DataStructuresForUnity.Runtime.GeneralUtils;

namespace GameplayAbilities.Runtime.Abilities {
    public abstract class SpawnableAbilityObject : PoolableObject, ISpawnable<AbilityData> {
        public abstract void Activate(AbilityData info);
        public abstract void Destroy();
    }
}
