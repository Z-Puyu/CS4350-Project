namespace GameplayAbilities.Runtime.Attributes {
    public readonly struct Attribute {
        public string Id { get; }
        public int Value { get; }
        public int MaxValue { get; }
        public int MinValue { get; }
        
        public Attribute(string id, int value, int minValue, int maxValue) {
            this.Id = id;
            this.Value = value;
            this.MaxValue = maxValue;
            this.MinValue = minValue;
        }
    }
}
