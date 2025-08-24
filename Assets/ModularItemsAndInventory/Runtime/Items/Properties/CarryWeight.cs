using SaintsField;
using UnityEngine;

namespace ModularItemsAndInventory.Runtime.Items.Properties {
    public sealed class CarryWeight : ItemProperty {
        [field: SerializeField, MinValue(0)] public int Weight { get; private set; }
        
        protected override int CompareToSameType(IItemProperty otherOfSameType) {
            CarryWeight weight = (CarryWeight)otherOfSameType;
            return this.Weight.CompareTo(weight.Weight);
        }
        
        public override IItemProperty Instantiate() {
            return new CarryWeight { Weight = this.Weight };
        }
    }
}
