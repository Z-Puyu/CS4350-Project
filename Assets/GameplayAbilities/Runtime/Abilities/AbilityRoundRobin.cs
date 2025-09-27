using System;
using System.Collections.Generic;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilities.Runtime.Abilities {
    [RequireComponent(typeof(AbilitySystem), typeof(AttributeSet)), DisallowMultipleComponent]
    public sealed class AbilityRoundRobin : MonoBehaviour {
        [field: SerializeField] private List<Ability> TestAbilities { get; set; } = new List<Ability>();

        private List<(IAbility ability, LinkedListNode<IAbility> node)> AbilityList { get; } =
            new List<(IAbility, LinkedListNode<IAbility>)>();

        private LinkedList<IAbility> EquippedAbilities { get; } = new LinkedList<IAbility>();
        private AbilitySystem AbilitySystem { get; set; }
        public IAbility Focus => this.EquippedAbilities.First?.Value;

        [field: SerializeField]
        private UnityEvent<IEnumerable<IAbility>> OnEquippedAbilitiesChanged { get; set; } =
            new UnityEvent<IEnumerable<IAbility>>();

        private void Awake() {
            this.AbilitySystem = this.GetComponent<AbilitySystem>();
        }

        private void Start() {
            this.SetCapacity(this.TestAbilities.Count);
            this.TestAbilities.ForEach(ability => this.Equip(ability, this.EquippedAbilities.Count - 1));
        }

        public void SetCapacity(int capacity) {
            if (capacity < 0) {
#if DEBUG
                Debug.LogError($"Capacity {capacity} is less than 0!", this);
#endif
            }

            if (capacity == this.AbilityList.Count) {
                return;
            }

            while (this.AbilityList.Count > capacity) {
                this.EquippedAbilities.Remove(this.AbilityList[^1].node);
                this.AbilityList.RemoveAt(this.AbilityList.Count - 1);
            }

            while (this.EquippedAbilities.Count < capacity) {
                LinkedListNode<IAbility> node = new LinkedListNode<IAbility>(null);
                this.EquippedAbilities.AddLast(node);
                this.AbilityList.Add((null, node));
            }

            this.OnEquippedAbilitiesChanged.Invoke(this.EquippedAbilities);
        }

        public void Equip(IAbility ability, int index) {
            if (index < 0 || index >= this.AbilityList.Count) {
#if DEBUG
                Debug.LogError($"Index {index} out of bounds!", this);
#endif
                return;
            }
            
            LinkedListNode<IAbility> node = this.AbilityList[index].node;
            LinkedListNode<IAbility> newNode = this.EquippedAbilities.AddBefore(node, ability);
            this.AbilityList[index] = (ability, newNode);
            this.EquippedAbilities.Remove(node);
            this.OnEquippedAbilitiesChanged.Invoke(this.EquippedAbilities);
        }

        public void Unequip(int index) {
            if (index < 0 || index >= this.AbilityList.Count) {
#if DEBUG
                Debug.LogError($"Index {index} out of bounds!", this);
#endif
            } else {
                this.EquippedAbilities.Remove(this.AbilityList[index].node);
                this.AbilityList[index] = (this.AbilityList[index].ability, null);
                this.OnEquippedAbilitiesChanged.Invoke(this.EquippedAbilities);
            }
        }

        public void RotateClockwise() {
            LinkedListNode<IAbility> node = this.EquippedAbilities.Last;
            this.EquippedAbilities.RemoveLast();
            this.EquippedAbilities.AddFirst(node);
        }

        public void RotateCounterClockwise() {
            LinkedListNode<IAbility> node = this.EquippedAbilities.First;
            this.EquippedAbilities.RemoveFirst();
            this.EquippedAbilities.AddLast(node);
        }

        public void UseCurrentAbility() {
            if (this.Focus == null) {
                return;
            }

#if DEBUG
            Debug.Log($"Using {this.Focus.Info.Id}", this);
#endif
            GameplayEffectExecutionArgs args = this.AbilitySystem.CreateEffectExecutionArgs().Build();
            this.AbilitySystem.Use(this.Focus, this.AbilitySystem, args);
        }

        public void UseCurrentAbility(Vector3 target) {
            if (this.Focus == null) {
                return;
            }

#if DEBUG
            Debug.Log($"Using {this.Focus.Info.Id}", this);
#endif
            GameplayEffectExecutionArgs args = this.AbilitySystem.CreateEffectExecutionArgs().Build();
            this.AbilitySystem.Use(this.Focus, this.AbilitySystem, args);
        }

        public void UseCurrentAbility(Transform target) {
            if (this.Focus == null) {
                return;
            }

#if DEBUG
            Debug.Log($"Using {this.Focus.Info.Id}", this);
#endif
            GameplayEffectExecutionArgs args = this.AbilitySystem.CreateEffectExecutionArgs().Build();
            this.AbilitySystem.Use(this.Focus, this.AbilitySystem, args);
        }
    }
}
