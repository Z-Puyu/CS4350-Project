using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using DataStructuresForUnity.Runtime.GeneralUtils;
using GameplayAbilities.Runtime.Abilities;
using UnityEngine;

namespace Visuals {
    [RequireComponent(typeof(ParticleSystem), typeof(BoundingRect))]
    public abstract class ParticleVisual2D : SpawnableAbilityObject {
        [field: SerializeField] private string Id { get; set; }
        protected ParticleSystem ParticleSystem { get; set; }
        protected BoundingRect BoundingRect { get; set; }
        protected BoundingRect ParentRect { get; set; }

        public override string PoolableId => this.Id;

        protected virtual void Awake() {
            this.ParticleSystem = this.GetComponent<ParticleSystem>();
            this.BoundingRect = this.GetComponent<BoundingRect>();
        }

        public override void Return() {
            this.ParticleSystem.Stop();
            this.StartCoroutine(this.WaitAndReturn());
        }

        private IEnumerator WaitAndReturn() {
            yield return new WaitForSeconds(this.ParticleSystem.main.duration - this.ParticleSystem.time);
            yield return new WaitUntil(() => !this.ParticleSystem.IsAlive());
            base.Return();
        }

        public override void Activate(AbilityData info) {
            Transform parent = this.transform.parent;
            if (parent) {
                this.ParentRect = this.transform.parent.GetComponentInParent<BoundingRect>();
            }
            
            this.BoundingRect.ResizeTo(this.ParentRect);
        }

        protected IEnumerator AlignToParentAndPlay(BoundingRect.Alignment alignment, AbilityData info) {
            yield return new WaitForFixedUpdate();
            this.BoundingRect.AlignTo(this.ParentRect, alignment);
            this.ParticleSystem.Play();
            yield return new WaitForSeconds(info.Info.Duration);
            yield return this.WaitToDestroy();
        }

        private IEnumerator WaitToDestroy() {
            if (this.ParticleSystem.main.loop) {
                this.ParticleSystem.Stop();
            }
            
            yield return new WaitUntil(() => !this.ParticleSystem.IsAlive());
            this.Return();
        }

        public override void Destroy() {
            this.StartCoroutine(this.WaitToDestroy());
        }
    }
}
