using UnityEngine;

namespace GameplayAbilities.Runtime.ParticleUtils {
    [DisallowMultipleComponent]
    public sealed class ParticleAttractionController : MonoBehaviour, IParticleController {
        public void Activate(ParticleSystem system, ParticleControllerSettings settings) {
            settings.Attractor.length = Vector3.Distance(this.transform.position, system.transform.position);
        }
    }
}
