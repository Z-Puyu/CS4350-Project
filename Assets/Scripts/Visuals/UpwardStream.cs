using System.Collections;
using System.Collections.Generic;
using Common;
using GameplayAbilities.Runtime.Abilities;
using UnityEngine;

namespace Visuals {
    public sealed class UpwardStream : ParticleVisual2D {
        public override void Activate(AbilityInfo info) {
            base.Activate(info);
            this.StartCoroutine(this.AlignToParentAndPlay(BoundingRect.Alignment.Bottom, info));
        }
    }
}
