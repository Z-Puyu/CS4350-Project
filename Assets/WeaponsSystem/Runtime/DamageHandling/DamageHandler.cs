using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.Runtime.DamageHandling {
    [DisallowMultipleComponent]
    public sealed class DamageHandler : MonoBehaviour {
        [field: SerializeReference] IAbility DamageAbility { get; set; }
        [field: SerializeField, Required] private AttributeSet AttributeSet { get; set; }
        
        public void Handle(Damage damage) {
            this.DamageAbility.Execute(new ReadonlyAttributes(damage), this.AttributeSet);
        }
    }
}
