using GameplayAbilities.Runtime.Attributes;
using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities {
    public readonly struct AbilityData {
        public AbilityInfo Info { get; }
        public Vector2 MousePosition { get; }
        public IAttributeReader Source { get; }
        
        public AbilityData(AbilityInfo info, IAttributeReader source, Vector2 mousePosition) {
            this.Info = info;
            this.Source = source;
            this.MousePosition = mousePosition;
        }
    }
}
