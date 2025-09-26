using Common;
using DataStructuresForUnity.Runtime.GeneralUtils;
using UnityEngine;

namespace Visuals {
    [DisallowMultipleComponent, RequireComponent(typeof(BoundingRect))]
    public sealed class VisualSpawner : MonoBehaviour {
        public void SpawnParticleVisual(ParticleVisual2D visual) {
            ParticleVisual2D instance = ObjectSpawner.Pull(visual.PoolableId, visual, this.transform);
            instance.Activate();
        }
    }
}
