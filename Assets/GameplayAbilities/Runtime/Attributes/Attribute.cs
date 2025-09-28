namespace GameplayAbilities.Runtime.Attributes {
    public readonly struct Attribute {
        public string Id { get; }
        public int Value { get; }
        
        public Attribute(string id, int value) {
            this.Id = id;
            this.Value = value;
        }
    }
}
