using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Targeting;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities {
    [DisallowMultipleComponent, RequireComponent(typeof(AttributeSet), typeof(AbilitySystem), typeof(AbilityTargeter))]
    public sealed class AbilityCaster : MonoBehaviour {
        private AttributeSet AttributeSet { get; set; }
        private AbilitySystem AbilitySystem { get; set; }
        private AbilityTargeter AbilityTargeter { get; set; }
        [field: SerializeField, MinValue(1)] private int AbilitySlots { get; set; }
        [field: SerializeField] private Ability TestCharacterAbility { get; set; }
        private List<Ability> EquippedAbilities { get; } = new List<Ability>();
        public int ReadiedSkillIndex { get; set; }

        private void Awake() {
            this.AttributeSet = this.GetComponent<AttributeSet>();
            this.AbilitySystem = this.GetComponent<AbilitySystem>();
            this.AbilityTargeter = this.GetComponent<AbilityTargeter>();
            for (int i = 0; i < this.AbilitySlots; i += 1) {
                this.EquippedAbilities.Add(null);
            }
        }

        private void Start() {
#if DEBUG
            if (this.TestCharacterAbility) {
                this.Equip(this.TestCharacterAbility, 1);
            }
#endif
        }

        public void Equip(Ability ability, int index) {
            this.EquippedAbilities[index] = ability;
        }

        [Button("Test Cast")]
        public void Cast(int index, AttributeSet target = null) {
            if (index < 0 || index >= this.AbilitySlots) {
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
            if (index < 0 || index >= this.AbilitySlots) {
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
            this.ReadiedSkillIndex = this.EquippedAbilities.IndexOf(ability);
        }

        public void ReleaseAbilitiesToCarriers(IEnumerable<IAbility> abilities, IEnumerable<GameObject> carriers) {
            IEnumerable<GameObject> gameObjects = carriers.ToArray();
            foreach (IAbility ability in abilities) {
                foreach (GameObject carrier in gameObjects) {
                    ability.Delegate(carrier, this, this.AbilityTargeter);
                }
            }
        }
    }
}
