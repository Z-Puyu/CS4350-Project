using System;
using System.Text;
using ModularItemsAndInventory.Runtime.Items;
using ModularItemsAndInventory.Runtime.Items.Properties;
using UnityEngine;

namespace Game.Items {
    [Serializable]
    public sealed class Plantable : ItemProperty {
        [field: SerializeField] private int GrowthDuration { get; set; }
        [field: SerializeField] private int WateringRequirement { get; set; }
        
        protected override string Encode() {
            StringBuilder sb = new StringBuilder(this.GetType().FullName);
            sb.Append($"-GrowthDuration:{this.GrowthDuration}");
            sb.Append($"-WateringRequirement:{this.WateringRequirement}");
            return sb.ToString();
        }

        public override IItemProperty Instantiate() {
            return new Plantable {
                GrowthDuration = this.GrowthDuration, 
                WateringRequirement = this.WateringRequirement
            };
        }

        public override void Process(in Item item, GameObject target) {
            // TODO: Implement this if anything needs to interact with a plantable item
            // example: a plot of land may take in a seed item and instantiate a plant on it
        }
    }
}
