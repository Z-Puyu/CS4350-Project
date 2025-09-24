namespace GameplayAbilities.Runtime.Abilities {
    public readonly struct AbilityInfo {
        public int Cooldown { get; }
        public int NumberOfEffects { get; }
        
        public AbilityInfo(int cooldown, int numberOfEffects) {
            this.Cooldown = cooldown;
            this.NumberOfEffects = numberOfEffects;
        }
    }
}
