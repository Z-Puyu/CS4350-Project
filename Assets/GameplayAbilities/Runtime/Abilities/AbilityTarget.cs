using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities {
    public class AbilityTarget : MonoBehaviour, IAbilityTarget {
        [field: SerializeField, Required] private AttributeSet AttributeSet { get; set; }
        
        public void Receive(IAbility ability, IAttributeReader from) {
            ability.Execute(from, this.AttributeSet);
        }
    }
}
