using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilities.Runtime.HealthSystem {
    [DisallowMultipleComponent]
    public sealed class Health : MonoBehaviour {
        [field: SerializeField, Required] private AttributeSet Root { get; set; }
        [field: SerializeField] private AttributeType HealthAttribute { get; set; }
        [field: SerializeField] private UnityEvent OnDeathEvent { get; set; } = new UnityEvent();
        
        public int Value => this.Root.GetCurrent(this.HealthAttribute.Id);
        public int MaxValue => this.Root.GetMax(this.HealthAttribute.Id);
        public int MinValue => this.Root.GetMin(this.HealthAttribute.Id);
        
        public event UnityAction<(int old, int current)> OnHealthChanged; 
        
        private void Start() {
            this.Root.OnAttributeChanged += this.HandleAttributeChange;
        }

        public void Refill() {
            int amount = this.MaxValue - this.Value;
            Modifier modifier = new Modifier(amount, Modifier.Operation.Offset, this.HealthAttribute.Id);
            this.Root.AddModifier(modifier);
        }

        private void HandleAttributeChange(AttributeChange change) {
            if (change.AttributeName != this.HealthAttribute.Id) {
                return;
            }
            
            
            this.OnHealthChanged?.Invoke((change.OldValue, change.CurrentValue));
            if (change.CurrentValue <= 0) {
                this.OnDeathEvent.Invoke();
            }
        }
    }
}
