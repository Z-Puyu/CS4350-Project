using System;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.Modifiers {
    [Serializable]
    public class ModifierData {
        private enum ValueSource { Target, Instigator }
        
        [field: SerializeField, TableColumn("Target")] 
        public AttributeTypeDefinition TargetAttribute { get; private set; }
    
        [field: SerializeField] public Modifier.Operation Method { get; private set; }
        
        [field: SerializeField, TableColumn("Magnitude")] 
        private bool UseAttributeValue { get; set; }

        [field: SerializeField, TableColumn("Magnitude"), ShowIf(nameof(this.UseAttributeValue))]
        private ValueSource Source { get; set; } = ValueSource.Instigator;
        
        [field: SerializeField, TableColumn("Magnitude"), ShowIf(nameof(this.UseAttributeValue))]
        private AttributeTypeDefinition SourceAttribute { get; set; }
        
        [field: SerializeField, TableColumn("Magnitude"), HideIf(nameof(this.UseAttributeValue))] 
        private bool AllowSetByCaller { get; set; }
        
        [field: SerializeField, TableColumn("Magnitude"), HideIf(nameof(this.UseAttributeValue))] 
        public int Value { get; private set; }
        
        [field: SerializeField, TableColumn("Magnitude"), Tooltip("Used to identify the user-set modifier value.")]
        [field: HideIf(nameof(this.UseAttributeValue)), ShowIf(nameof(this.AllowSetByCaller))]
        public string Label { get; private set; }

        public Modifier CreateModifier(AttributeSet target, GameplayEffectExecutionArgs args) {
            if (this.UseAttributeValue) {
                int value = this.Source switch {
                    ValueSource.Target => target.GetCurrent(this.SourceAttribute.Id),
                    ValueSource.Instigator => args.Instigator.GetCurrent(this.SourceAttribute.Id),
                    var _ => throw new ArgumentException("Invalid value source")
                };
                
                return new Modifier(value, this.Method, this.TargetAttribute.Id);
            }

            if (this.AllowSetByCaller && args.CallerSuppliedModifierValues.TryGetValue(this.Label, out int val)) {
                return new Modifier(val, this.Method, this.TargetAttribute.Id);
            }
            
            return new Modifier(this.Value, this.Method, this.TargetAttribute.Id);
        }
    }
}
