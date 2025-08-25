using System.Collections.Generic;
using GameplayAbilities.Runtime.GameplayEffects;
using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities {
    [CreateAssetMenu(fileName = "New Ability", menuName = "Gameplay Abilities/Ability", order = 0)]
    public class Ability : ScriptableObject, IAbility {
        [field: SerializeField] public List<GameplayEffectData> Effects { get; set; } = new List<GameplayEffectData>();
        
        public bool IsUsable(AttributeSet instigator) {
            return true;
        }
    }
}
