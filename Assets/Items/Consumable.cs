using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.GameplayEffects;
using ModularItemsAndInventory.Runtime.Items;
using ModularItemsAndInventory.Runtime.Items.Properties;
using UnityEngine;

namespace Items {
    [Serializable]
    public class Consumable : ItemProperty {
        [field: SerializeField] private List<GameplayEffectData> Effects { get; set; } = new List<GameplayEffectData>();

        protected override string Encode() {
            StringBuilder sb = new StringBuilder(this.GetType().FullName);
            List<GameplayEffectData> effects = this.Effects.ToList();
            effects.Sort();
            foreach (GameplayEffectData effect in effects) {
                sb.AppendLine($"_Effect:{effect.SortKey}");
            }
            
            return sb.ToString();
        }

        public override IItemProperty Instantiate() {
            return new Consumable { Effects = this.Effects.ToList() };
        }
        
        public override void Process(in Item item, GameObject target) {
            if (target.TryGetComponent(out AbilitySystem system)) {
                this.Effects.ForEach(system.AddEffect);
            }
        }
        
        public override string ToString() {
            StringBuilder sb = new StringBuilder("Consumable property:\n");
            foreach (GameplayEffectData effect in this.Effects.OrderBy(effect => effect)) {
                sb.AppendLine($"- {effect}");
            }
            
            return sb.ToString();
        }
    }
}
