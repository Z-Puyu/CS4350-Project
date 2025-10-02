using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using DataStructuresForUnity.Runtime.Utilities;
using GameplayAbilities.Runtime.Abilities;
using UnityEngine;

namespace Visuals {
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleVisual2D : MonoBehaviour, IActivatable {
        private ParticleSystem ParticleSystem { get; set; }
        public bool IsActive { get; private set; }

        protected virtual void Awake() {
            this.ParticleSystem = this.GetComponent<ParticleSystem>();
        }

        private IEnumerator Play() {
            yield return new WaitForFixedUpdate();
            this.IsActive = true;
            this.ParticleSystem.Play();
        }

        private IEnumerator Stop() {
            if (this.ParticleSystem.main.loop) {
                this.ParticleSystem.Stop();
            } 
            
            yield return new WaitUntil(() => !this.ParticleSystem.IsAlive());
            this.IsActive = false;
        }

        public void Activate() {
            this.StartCoroutine(this.Play());
        }
        
        public void Deactivate() {
            this.StartCoroutine(this.Stop());
        }
    }
}
