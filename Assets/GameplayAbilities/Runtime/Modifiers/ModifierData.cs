using System;
using System.Text;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameplayAbilities.Runtime.Modifiers {
    [Serializable]
    public class ModifierData : IComparable<ModifierData>, IEquatable<ModifierData> {
        private Lazy<string> CachedSortKey { get; } 
        internal string SortKey => this.CachedSortKey.Value;
        
        private enum MagnitudeType { Constant, AttributeValue, CallerSupplied }
        
        private enum ValueSource { Target, Instigator }
        
        [field: SerializeField, TableColumn("Target"), Dropdown(nameof(this.AllAttributeOptions))] 
        public string TargetAttribute { get; private set; }
    
        [field: SerializeField] public Modifier.Operation Method { get; private set; }
        
        [field: SerializeField, TableColumn("Magnitude")] 
        private MagnitudeType Form { get; set; }
        
        private bool UseAttributeValue => this.Form == MagnitudeType.AttributeValue;
        internal bool AllowSetByCaller => this.Form == MagnitudeType.CallerSupplied;
        private bool UseConstant => this.Form == MagnitudeType.Constant;

        [field: SerializeField, TableColumn("Magnitude"), ShowIf(nameof(this.UseAttributeValue))]
        private ValueSource Source { get; set; } = ValueSource.Instigator;
        
        [field: SerializeField, TableColumn("Magnitude"), ShowIf(nameof(this.UseAttributeValue))]
        [field: TreeDropdown(nameof(this.AttributeOptions))]
        private string SourceAttribute { get; set; }
        
        [field: SerializeField, TableColumn("Magnitude"), HideIf(nameof(this.UseAttributeValue))] 
        public int DefaultValue { get; private set; }
        
        [field: SerializeField, HideIf(nameof(this.UseConstant))] 
        private float Coefficient { get; set; } = 1f;
        
        [field: SerializeField, TableColumn("Magnitude"), Tooltip("Used to identify the user-set modifier value.")]
        [field: ShowIf(nameof(this.AllowSetByCaller))]
        public string Label { get; private set; }
        
        private AdvancedDropdownList<string> AttributeOptions => this.GetAttributeOptions();
        private DropdownList<string> AllAttributeOptions => this.GetAllAttributes();

        private ModifierData() {
            this.CachedSortKey = new Lazy<string>(this.GenerateSortKey);
        }

        private string GenerateSortKey() {
            StringBuilder sb = new StringBuilder(this.GetType().FullName);
            sb.AppendLine($"-TargetAttribute:{this.TargetAttribute}");
            sb.AppendLine($"-Method:{this.Method}");
            sb.AppendLine($"-Form:{this.Form}");
            switch (this.Form) {
                case MagnitudeType.Constant:
                    sb.AppendLine($"-Magnitude:{this.DefaultValue}");
                    break;
                case MagnitudeType.AttributeValue:
                    sb.AppendLine($"-Source:{this.Source}");
                    sb.AppendLine($"-SourceAttribute:{this.SourceAttribute}");
                    sb.AppendLine($"-Coefficient:{this.Coefficient}");
                    break;
                case MagnitudeType.CallerSupplied:
                    sb.AppendLine($"-DefaultValue:{this.DefaultValue}");
                    sb.AppendLine($"-Label:{this.Label}");
                    sb.AppendLine($"-Coefficient:{this.Coefficient}");
                    break;
            }
            
            return sb.ToString();
        }

        public Modifier CreateModifier(AttributeSet target, GameplayEffectExecutionArgs args) {
            if (this.UseAttributeValue) {
                int value = this.Source switch {
                    ValueSource.Target => target.GetCurrent(this.SourceAttribute),
                    ValueSource.Instigator => args.Instigator.GetCurrent(this.SourceAttribute),
                    var _ => throw new ArgumentException("Invalid value source")
                };
                
                return new Modifier(value, this.Method, this.TargetAttribute) * this.Coefficient;
            }

            if (this.AllowSetByCaller && args.HasData(this.Label, out int val)) {
                return new Modifier(val, this.Method, this.TargetAttribute) * this.Coefficient;
            }
            
            return new Modifier(this.DefaultValue, this.Method, this.TargetAttribute);
        }

        public int CompareTo(ModifierData other) {
            if (other is null) {
                return 1;
            }

            return object.ReferenceEquals(this, other)
                    ? 0
                    : string.CompareOrdinal(this.SortKey, other.SortKey);
        }
        
        public bool Equals(ModifierData other) {
            return this.CompareTo(other) == 0;
        }

        public override int GetHashCode() {
            return this.SortKey.GetHashCode();
        }
    }
}
