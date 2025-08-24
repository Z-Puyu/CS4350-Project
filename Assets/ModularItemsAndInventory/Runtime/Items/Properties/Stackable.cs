using SaintsField;
using UnityEngine;

namespace ModularItemsAndInventory.Runtime.Items.Properties {
    public sealed class Stackable : ItemProperty {
        [field: SerializeField, MinValue(2)] public int StackLimit { get; private set; } = 2;
        
        protected override int CompareToSameType(IItemProperty otherOfSameType) {
            Stackable stackable = (Stackable)otherOfSameType;
            return stackable.StackLimit - this.StackLimit;
        }
        
        public override IItemProperty Instantiate() {
            return new Stackable { StackLimit = this.StackLimit };
        }
    }
}
