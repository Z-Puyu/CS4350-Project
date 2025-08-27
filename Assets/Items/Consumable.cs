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
    public class Consumable : ItemProperty<AbilitySystem> {
        [field: SerializeField] private List<GameplayEffectData> Effects { get; set; } = new List<GameplayEffectData>();
        
        public override IItemProperty Instantiate() {
            return new Consumable { Effects = this.Effects.ToList() };
        }
        
        protected override int CompareToSameType(IItemProperty otherOfSameType) {
            Consumable other = (Consumable)otherOfSameType;
            if (this.Effects.Count > other.Effects.Count) {
                return 1;
            }
            
            List<GameplayEffectData> sorted = this.Effects.OrderBy(effect => effect).ToList();
            List<GameplayEffectData> otherSorted = other.Effects.OrderBy(effect => effect).ToList();
            for (int i = 0; i < sorted.Count; i += 1) {
                int comparison = sorted[i].CompareTo(otherSorted[i]);
                if (comparison != 0) {
                    return comparison;
                }
            }
            
            return 0;
        }
        
        public override void Process(in Item item, AbilitySystem target) {
            this.Effects.ForEach(target.AddEffect);
        }
        
        public override string ToString() {
            StringBuilder sb = new StringBuilder("Consumable property:\n");
            foreach (GameplayEffectData effect in this.Effects.OrderBy(effect => effect)) {
                sb.AppendLine($"- {effect}");
            }
            
            return sb.ToString();
        }
        
        protected override int HashPropertyContents() {
            return this.ToString().GetHashCode();
        }
    }
}
