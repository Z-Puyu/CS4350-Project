using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities {
    [CreateAssetMenu(fileName = "New Ability", menuName = "Gameplay Abilities/Ability", order = 0)]
    public class Ability : ScriptableObject, IAbility {
        [field: SerializeField] internal string Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField, MinValue(0)] private int Cooldown { get; set; }
        [field: SerializeField] internal GameObject SpawnedEffect { get; private set; }
        
        [field: SerializeReference, ReferencePicker] 
        public List<GameplayEffectData> Effects { get; private set; } = new List<GameplayEffectData>();
        
        public AbilityInfo Info => new AbilityInfo(this.Cooldown, this.Effects.Count);
        
        public IEnumerable<GameplayEffect> GenerateEffects(GameplayEffectExecutionArgs args) {
            return this.Effects.Select(effect => new GameplayEffect(effect, args));
        }

        public bool IsUsable(AttributeSet instigator, AttributeSet target) {
            return true;
        }
    }
}

