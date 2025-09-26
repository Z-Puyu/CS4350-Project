namespace GameplayAbilities.Runtime.Abilities {
    public readonly struct AbilityData {
        public AbilityInfo Info { get; }
        
        public AbilityData(AbilityInfo info) {
            this.Info = info;
        }
    }
}
