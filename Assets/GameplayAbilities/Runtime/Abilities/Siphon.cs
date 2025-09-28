using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities {
    public sealed class Siphon : SpawnableAbilityObject {
        private List<ParticleSystem> Particles { get; } = new List<ParticleSystem>();
        
        protected override void Awake() {
            base.Awake();
            this.GetComponentsInChildren(this.Particles);
        }
        
        public override void OnActive(AbilityData data) {
            foreach (ParticleSystem particle in this.Particles) {
                NativeArray<ParticleSystem.Particle> particles =
                        new NativeArray<ParticleSystem.Particle>(particle.main.maxParticles, Allocator.Temp);
                particle.GetParticles(particles);
                for (int i = 0; i < particles.Length; i += 1) {
                    Vector3 toSource = (data.SourceTransform.position - particles[i].position).normalized;
                    
                }
            }
        }
    }
}
