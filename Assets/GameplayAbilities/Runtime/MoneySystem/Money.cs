using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilities.Runtime.MoneySystem
{
    [DisallowMultipleComponent]
    public class Money : MonoBehaviour
    {
        [field: SerializeField, Required] private AttributeSet Root { get; set; }
        [field: SerializeField] private AttributeType MoneyAttribute { get; set; }
        [field: SerializeField] private UnityEvent OnMoneyChangedEvent { get; set; } = new UnityEvent();

        public int Value => Root.GetCurrent(MoneyAttribute.Id);
        public int MaxValue => Root.GetMax(MoneyAttribute.Id);
        public int MinValue => Root.GetMin(MoneyAttribute.Id);

        public event UnityAction<(int old, int current)> OnMoneyChanged;

        private void Start()
        {
            Root.OnAttributeChanged += HandleAttributeChange;
        }

        public bool Spend(int amount)
        {
            if (Value < amount)
            {
                Debug.LogWarning($"Not enough money! Need {amount}, have {Value}.");
                return false;
            }

            Modifier modifier = new Modifier(-amount, Modifier.Operation.Offset, MoneyAttribute.Id);
            Root.AddModifier(modifier);
            return true;
        }

        public void Add(int amount)
        {
            Modifier modifier = new Modifier(amount, Modifier.Operation.Offset, MoneyAttribute.Id);
            Root.AddModifier(modifier);
        }

        private void HandleAttributeChange(AttributeChange change)
        {
            if (change.AttributeName != MoneyAttribute.Id)
                return;

            OnMoneyChanged?.Invoke((change.OldValue, change.CurrentValue));
            OnMoneyChangedEvent.Invoke();

#if DEBUG
            Debug.Log($"{transform.root.name}: Money changed from {change.OldValue} to {change.CurrentValue}", this);
#endif
        }
    }
}
