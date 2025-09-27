namespace GameplayAbilities.Runtime.Abilities {
    public readonly struct AbilityInfo {
        public string Id { get; }
        public int DurationPlusCooldown { get; }
        public int Cooldown { get; }
        public int Duration { get; }

        public AbilityInfo(string id, int cooldown, int duration) {
            this.Id = id;
            this.Cooldown = cooldown;
            this.Duration = duration;
            this.DurationPlusCooldown = cooldown + duration;
        }
    }
}
