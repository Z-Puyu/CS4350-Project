using System;
using System.Collections;
using System.Collections.Generic;
using DataStructuresForUnity.Runtime.GeneralUtils;
using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities {
    [DisallowMultipleComponent]
    public sealed class AbilityVisualEffect : PoolableObject {
        [field: SerializeField] private string Id { get; set; }
        private List<ParticleSystem> Particles { get; } = new List<ParticleSystem>();
        
        public override string PoolableId => this.Id;

        private void Awake() {
            this.GetComponentsInChildren(this.Particles);
        }

        private void OnEnable() {
            foreach (ParticleSystem particle in this.Particles) {
                particle.Play();
            }
        }
        
        public void Stop() {
            foreach (ParticleSystem particle in this.Particles) {
                particle.Stop();
            }

            this.StartCoroutine(this.WaitToDestroy());
        }

        private IEnumerator WaitToDestroy() {
            yield return new WaitUntil(() => this.Particles.TrueForAll(particle => particle.particleCount == 0));
            this.Return();
        }
    }
}
