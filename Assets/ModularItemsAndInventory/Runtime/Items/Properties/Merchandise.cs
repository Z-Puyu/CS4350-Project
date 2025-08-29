using SaintsField;
using UnityEngine;

namespace ModularItemsAndInventory.Runtime.Items.Properties {
    public sealed class Merchandise : ItemProperty {
        [field: SerializeField] public int Price { get; private set; }
        [field: SerializeField] private bool HasDifferentPriceForSale { get; set; }
        
        [field: SerializeField, ShowIf(nameof(this.HasDifferentPriceForSale))] 
        [field: OnValueChanged(nameof(this.UnifyPrices))]
        public int Worth { get; private set; }

        private void UnifyPrices(object hasDifferentPriceForSale) {
            if (!(bool)hasDifferentPriceForSale) {
                this.Worth = this.Price;
            }
        }

        protected override string GenerateSortKey() {
            return $"{this.GetType().FullName}_Price:{this.Price}_Worth:{this.Worth}";
        }

        public override IItemProperty Instantiate() {
            return new Merchandise {
                Worth = this.Worth,
                Price = this.Price,
                HasDifferentPriceForSale = this.HasDifferentPriceForSale
            };
        }

        public override void Process(in Item item, GameObject target) { }
    }
}
