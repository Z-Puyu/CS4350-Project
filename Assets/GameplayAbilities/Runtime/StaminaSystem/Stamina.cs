using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilities.Runtime.StaminaSystem
{
    [DisallowMultipleComponent]
    public sealed class Stamina : MonoBehaviour
    {
        [field: SerializeField, Required] private AttributeSet Root { get; set; }
        [field: SerializeField] private AttributeType StaminaAttribute { get; set; }
        [field: SerializeField] private UnityEvent OnStaminaDepleted { get; set; } = new UnityEvent();
        [field: SerializeField] private float RegenRatePerSecond { get; set; } = 10f;
        [field: SerializeField] private float RegenDelay { get; set; } = 1.5f; // delay after use before regen starts

        private float lastConsumeTime;
        
        private float regenAccumulator = 0f;

        public int Value => this.Root.GetCurrent(this.StaminaAttribute.Id);
        public int MaxValue => this.Root.GetMax(this.StaminaAttribute.Id);
        public int MinValue => this.Root.GetMin(this.StaminaAttribute.Id);

        public event UnityAction<(int old, int current)> OnStaminaChanged;

        private void Start()
        {
            this.Root.OnAttributeChanged += this.HandleAttributeChange;
        }

        private void Update()
        {
            RegenerateStamina();
        }

        // --- Core Methods ---

        public bool HasEnough(int amount) => this.Value >= amount;

        public void Consume(int amount)
        {
            if (amount <= 0)
                return;

            int newValue = Mathf.Max(this.Value - amount, this.MinValue);
            int delta = newValue - this.Value;

            Modifier mod = new Modifier(delta, Modifier.Operation.Offset, this.StaminaAttribute.Id);
            this.Root.AddModifier(mod);

            lastConsumeTime = Time.time;
            if (newValue <= 0)
                this.OnStaminaDepleted.Invoke();
        }

        public void Refill()
        {
            int amount = this.MaxValue - this.Value;
            Modifier mod = new Modifier(amount, Modifier.Operation.Offset, this.StaminaAttribute.Id);
            this.Root.AddModifier(mod);
        }


        private void RegenerateStamina()
        {
            if (Time.time - lastConsumeTime < RegenDelay)
                return;

            float regenAmount = RegenRatePerSecond * Time.deltaTime;
            regenAccumulator += regenAmount;

            if (regenAccumulator >= 1f)
            {
                int intAmount = Mathf.FloorToInt(regenAccumulator);
                Modifier mod = new Modifier(intAmount, Modifier.Operation.Offset, this.StaminaAttribute.Id);
                this.Root.AddModifier(mod);
                regenAccumulator -= intAmount;
            }
        }

        private void HandleAttributeChange(AttributeChange change)
        {
            if (change.AttributeName != this.StaminaAttribute.Id)
                return;

            this.OnStaminaChanged?.Invoke((change.OldValue, change.CurrentValue));

#if DEBUG
            Debug.Log($"{this.transform.root.name}: Stamina changed from {change.OldValue} to {change.CurrentValue}", this);
#endif
        }
    }
}
