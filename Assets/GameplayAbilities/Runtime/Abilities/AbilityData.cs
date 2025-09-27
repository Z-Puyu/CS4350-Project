using GameplayAbilities.Runtime.Attributes;
using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities {
    public readonly struct AbilityData {
        public AbilityInfo Info { get; }
        public Vector3 Position { get; }
        public IAttributeReader Source { get; }
        
        public AbilityData(AbilityInfo info, IAttributeReader source, Vector3 position) {
            this.Info = info;
            this.Source = source;
            this.Position = position;
        }
    }
}
