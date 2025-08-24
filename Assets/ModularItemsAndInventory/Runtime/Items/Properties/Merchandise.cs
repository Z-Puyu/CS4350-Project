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
        
        protected override int CompareToSameType(IItemProperty otherOfSameType) {
            Merchandise merchandise = (Merchandise)otherOfSameType;
            return (this.Worth + this.Price).CompareTo(merchandise.Worth + merchandise.Price);
        }
        
        public override IItemProperty Instantiate() {
            return new Merchandise {
                Worth = this.Worth,
                Price = this.Price,
                HasDifferentPriceForSale = this.HasDifferentPriceForSale
            };
        }
    }
}
