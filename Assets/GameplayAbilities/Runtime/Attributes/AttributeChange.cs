namespace GameplayAbilities.Runtime.Attributes {
    public readonly struct AttributeChange {
        public string AttributeName { get; }
        public int OldValue { get; }
        public int CurrentValue { get; }
        
        public AttributeChange(string attributeName, int oldValue, int currentValue) {
            this.AttributeName = attributeName;
            this.OldValue = oldValue;
            this.CurrentValue = currentValue;
        }
    }
}
