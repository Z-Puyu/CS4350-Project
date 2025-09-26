namespace GameplayAbilities.Runtime.Abilities {
    public readonly struct AbilityInfo {
        public string Id { get; }
        public int Cooldown { get; }
        public int NumberOfEffects { get; }
        
        public AbilityInfo(string id, int cooldown, int numberOfEffects) {
            this.Id = id;
            this.Cooldown = cooldown;
            this.NumberOfEffects = numberOfEffects;
        }
    }
}
