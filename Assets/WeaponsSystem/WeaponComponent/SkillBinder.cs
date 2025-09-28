using System;
using GameplayAbilities.Runtime.Abilities;
using UnityEngine;
using WeaponsSystem.DamageHandling;

namespace WeaponsSystem.WeaponComponent {
    public class SkillBinder : MonoBehaviour {
        [field: SerializeField] public SkillEvent skillEvent;
        [field: SerializeField] private AbilitySystem abilitySystem;
        [field: SerializeField] private Combatant combatant;

        public void OnEnable() {
            this.skillEvent.onSkillActivatable.AddListener(this.abilitySystem.Grant);
        }

        public void OnDisable() {
            this.skillEvent.onSkillActivatable.RemoveListener(this.abilitySystem.Grant);
        }
    }
}
