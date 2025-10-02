using UnityEngine;

namespace GameplayAbilities.Runtime.ParticleUtils {
    public readonly struct ParticleControllerSettings {
        public ParticleSystemForceField Attractor { get; }

        public ParticleControllerSettings(ParticleSystemForceField attractor) {
            this.Attractor = attractor;
        }
    }
}
