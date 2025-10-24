using System;
using System.Text;
using ModularItemsAndInventory.Runtime.Items;
using ModularItemsAndInventory.Runtime.Items.Properties;
using Player_related.Player_exp;
using UnityEngine;

namespace Game.Items {
    [Serializable]
    public sealed class Plantable : ItemProperty {
        [field: SerializeField] public int GrowthDuration { get; private set; }
        [field: SerializeField] public int WateringRequirement { get; private set; }
        [field: SerializeField] public FarmingExpObject FarmingExpObject { get; private set; }
        [field: SerializeField] public float AttackBuff { get; private set; }

        protected override string Encode() {
            StringBuilder sb = new StringBuilder(this.GetType().FullName);
            sb.Append($"-GrowthDuration:{this.GrowthDuration}");
            sb.Append($"-WateringRequirement:{this.WateringRequirement}");
            return sb.ToString();
        }

        public override IItemProperty Instantiate() {
            return new Plantable {
                GrowthDuration = this.GrowthDuration, 
                WateringRequirement = this.WateringRequirement,
                FarmingExpObject = this.FarmingExpObject,
                AttackBuff = this.AttackBuff
            };
        }

        public override void Process(in Item item, GameObject target) {
            // Example: handle planting logic here later
        }
    }
}
