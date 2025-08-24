using System;
using UnityEngine;

namespace GameplayAttributes.Runtime.Modifiers {
    [Serializable]
    public struct AttributeValue : IModifierMagnitude {
        private enum Source { Target, Instigator }
        
        [field: SerializeField] private Source SourceAttributeSet { get; set; }
        [field: SerializeField] private AttributeTypeDefinition Attribute { get; set; }
        [field: SerializeField] private float Coefficient { get; set; }
        
        float IModifierMagnitude.Evaluate(AttributeSet target, IAttributeReader instigator) {
            return this.SourceAttributeSet switch {
                Source.Target => target.GetCurrent(this.Attribute.Id),
                Source.Instigator => instigator.GetCurrent(this.Attribute.Id),
                var _ => throw new ArgumentOutOfRangeException()
            } * this.Coefficient;
        }
    }
}
