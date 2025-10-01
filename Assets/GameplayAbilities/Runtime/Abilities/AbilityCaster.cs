using System.Collections.Generic;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Targeting;
using SaintsField.Playa;
using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities {
    [DisallowMultipleComponent, RequireComponent(typeof(AttributeSet), typeof(AbilitySystem), typeof(AbilityTargeter))]
    public sealed class AbilityCaster : MonoBehaviour {
        private AttributeSet AttributeSet { get; set; }
        private AbilitySystem AbilitySystem { get; set; }
        private AbilityTargeter AbilityTargeter { get; set; }
        [field: SerializeField] private List<Ability> EquippedAbilities { get; set; } = new List<Ability>();

        private void Awake() {
            this.AttributeSet = this.GetComponent<AttributeSet>();
            this.AbilitySystem = this.GetComponent<AbilitySystem>();
        }

        public void Equip(Ability ability, int index) {
            this.EquippedAbilities[index] = ability;
        }

        [Button("Test Cast")]
        public void Cast(int index, AttributeSet target = null) {
            if (index < 0 || index >= this.EquippedAbilities.Count) {
#if DEBUG
                Debug.LogError($"Index {index} out of bounds!", this);
#endif
                return;
            }
            
            if (!this.EquippedAbilities[index]) {
#if DEBUG
                Debug.LogError($"No ability at index {index}!", this);
#endif
                return;
            }
            
            this.Cast(this.EquippedAbilities[index], target);
        }
        
        public void Cast(Ability ability, AttributeSet target = null) {
            if (!target) {
                target = this.AttributeSet;
            }
            
            ability.Execute(this.AttributeSet, target);
        }

        public void Ready(int index) {
            if (index < 0 || index >= this.EquippedAbilities.Count) {
#if DEBUG
                Debug.LogError($"Index {index} out of bounds!", this);
#endif
                return;
            }

            if (!this.EquippedAbilities[index]) {
#if DEBUG
                Debug.LogError($"No ability at index {index}!", this);
#endif
                return;
            }
            
            this.Ready(this.EquippedAbilities[index]);
        }

        public void Ready(Ability ability) {
            ability.Activate(this, this.AbilityTargeter);
        }
    }
}
