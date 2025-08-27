using System;
using System.Text;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using SaintsField;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameplayAbilities.Runtime.Modifiers {
    [Serializable]
    public class ModifierData : IComparable<ModifierData>, IEquatable<ModifierData> {
        private enum MagnitudeType { Constant, AttributeValue, CallerSupplied }
        
        private enum ValueSource { Target, Instigator }
        
        [field: SerializeField, TableColumn("Target")] 
        public AttributeTypeDefinition TargetAttribute { get; private set; }
    
        [field: SerializeField] public Modifier.Operation Method { get; private set; }
        
        [field: SerializeField, TableColumn("Magnitude")] 
        private MagnitudeType Form { get; set; }
        
        private bool UseAttributeValue => this.Form == MagnitudeType.AttributeValue;
        private bool AllowSetByCaller => this.Form == MagnitudeType.CallerSupplied;
        private bool UseConstant => this.Form == MagnitudeType.Constant;

        [field: SerializeField, TableColumn("Magnitude"), ShowIf(nameof(this.UseAttributeValue))]
        private ValueSource Source { get; set; } = ValueSource.Instigator;
        
        [field: SerializeField, TableColumn("Magnitude"), ShowIf(nameof(this.UseAttributeValue))]
        private AttributeTypeDefinition SourceAttribute { get; set; }
        
        [field: SerializeField, TableColumn("Magnitude"), HideIf(nameof(this.UseAttributeValue))] 
        public int DefaultValue { get; private set; }
        
        [field: SerializeField, TableColumn("Magnitude"), Tooltip("Used to identify the user-set modifier value.")]
        [field: ShowIf(nameof(this.AllowSetByCaller))]
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
            
            return new Modifier(this.DefaultValue, this.Method, this.TargetAttribute.Id);
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder("Modifier:\n");
            sb.AppendLine($"Target: {this.TargetAttribute.Id}");
            sb.AppendLine($"Modification method: {this.Method}");
            sb.AppendLine($"Modifier value type: {this.Form}");
            switch (this.Form) {
                case MagnitudeType.Constant:
                    sb.AppendLine($"Magnitude: {this.DefaultValue}");
                    break;
                case MagnitudeType.AttributeValue:
                    sb.AppendLine($"Magnitude based on {this.SourceAttribute.Id} from {this.Source}");
                    break;
                case MagnitudeType.CallerSupplied:
                    sb.AppendLine($"Magnitude: set by caller as {this.Label}, or {this.DefaultValue} by default");
                    break;
            }
            
            return sb.ToString();
        }

        public int CompareTo(ModifierData other) {
            return object.ReferenceEquals(this, other) ? 0 : string.CompareOrdinal(this.ToString(), other.ToString());
        }
        
        public bool Equals(ModifierData other) {
            return this.CompareTo(other) == 0;
        }

        public override int GetHashCode() {
            return this.ToString().GetHashCode();
        }
    }
}
