using GameplayAbilities.Runtime.Attributes;

namespace GameplayAbilities.Runtime.Abilities {
    public readonly struct AbilityData {
        public AbilityInfo Info { get; }
        public IAttributeReader Source { get; }
        
        public AbilityData(AbilityInfo info, IAttributeReader source) {
            this.Info = info;
            this.Source = source;
        }
    }
}
