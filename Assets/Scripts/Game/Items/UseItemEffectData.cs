using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using GameplayAbilities.Runtime.Modifiers;
using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items;
using UnityEngine;

namespace Game.Items {
    public sealed class UseItemEffectData : GameplayEffectData {
        [field: SerializeField] private string DataLabel { get; set; }
        
        public override IEnumerable<Modifier> Run(AttributeSet target, GameplayEffectExecutionArgs args) {
            if (args.HasData(this.DataLabel, out Item item) &&
                item.Properties.Have(out IEnumerable<Consumable> consumables)) {
                return consumables.SelectMany(consumable =>
                        consumable.Effects.SelectMany(effect => effect.Run(target, args))
                );
            }

            return Enumerable.Empty<Modifier>();
        }
    }
}
