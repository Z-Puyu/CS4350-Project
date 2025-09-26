using DataStructuresForUnity.Runtime.GeneralUtils;

namespace GameplayAbilities.Runtime.Abilities {
    public abstract class SpawnableAbilityObject : PoolableObject, ISpawnable {
        public abstract void Activate();
        public abstract void Destroy();
    }
}
