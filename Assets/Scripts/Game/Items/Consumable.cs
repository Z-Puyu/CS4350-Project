using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using ModularItemsAndInventory.Runtime.Items;
using ModularItemsAndInventory.Runtime.Items.Properties;
using UnityEngine;

namespace Game.Items {
    [Serializable]
    public class Consumable : ItemProperty {
        [field: SerializeReference]
        public List<GameplayEffectData> Effects { get; private set; } = new List<GameplayEffectData>();
        
        protected override string Encode() {
            StringBuilder sb = new StringBuilder(this.GetType().FullName);
            List<GameplayEffectData> effects = this.Effects.ToList();
            effects.Sort();
            foreach (GameplayEffectData effect in effects) {
                sb.AppendLine($"-Effect:{effect.SortKey}");
            }
            
            return sb.ToString();
        }

        public override IItemProperty Instantiate() {
            return new Consumable { Effects = this.Effects.ToList() };
        }

        public override void Process(in Item item, GameObject target) {
            GameplayEffectCoordinator coordinator = target.GetComponentInChildren<GameplayEffectCoordinator>();
            if (!coordinator) {
                return;
            }
            
            GameplayEffectExecutionArgs args = coordinator.CreateEffectExecutionArgs().Build();
            foreach (GameplayEffectData effect in this.Effects) {
                coordinator.Add(effect.Instantiate(coordinator.GetComponent<AttributeSet>(), args), 100);
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
