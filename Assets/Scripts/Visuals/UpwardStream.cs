using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Visuals {
    public sealed class UpwardStream : ParticleVisual2D {
        public override void Activate() {
            base.Activate();
            this.StartCoroutine(this.AlignToParent());
        }
        
        private IEnumerator AlignToParent() {
            yield return new WaitForEndOfFrame();
            yield return new WaitForFixedUpdate();
            this.BoundingRect.AlignTo(this.ParentRect, BoundingRect.Alignment.Bottom);
            yield return new WaitForEndOfFrame();
            this.ParticleSystem.Play();
        }
    }
}
