using SaintsField;
using UnityEngine;

namespace ModularItemsAndInventory.Runtime.Items.Properties {
    public sealed class CarryWeight : ItemProperty {
        [field: SerializeField, MinValue(0)] public int Weight { get; private set; }

        protected override string GenerateSortKey() {
            return $"{this.GetType().FullName}_Weight:{this.Weight}";
        }

        public override IItemProperty Instantiate() {
            return new CarryWeight { Weight = this.Weight };
        }

        public override void Process(in Item item, GameObject target) { }
    }
}
