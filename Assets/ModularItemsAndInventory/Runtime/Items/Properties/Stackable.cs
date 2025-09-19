using SaintsField;
using UnityEngine;

namespace ModularItemsAndInventory.Runtime.Items.Properties {
    public sealed class Stackable : ItemProperty {
        [field: SerializeField, MinValue(2)] public int StackLimit { get; private set; } = 2;
        
        protected override string Encode() {
            return $"{this.GetType().FullName}_StackLimit:{this.StackLimit}";
        }

        public override IItemProperty Instantiate() {
            return new Stackable { StackLimit = this.StackLimit };
        }

        public override void Process(in Item item, GameObject target) { }
    }
}
