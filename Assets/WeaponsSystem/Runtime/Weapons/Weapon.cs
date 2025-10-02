using System;
using System.Collections.Generic;
using DataStructuresForUnity.Runtime.Utilities;
using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.Attributes;
using Projectiles.Runtime;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;
using WeaponsSystem.Runtime.Attacks;
using WeaponsSystem.Runtime.WeaponComponents;

namespace WeaponsSystem.Runtime.Weapons {
    [DisallowMultipleComponent]
    public class Weapon : MonoBehaviour {
        [field: SerializeField, Required] private AttributeSet Stats { get; set; }

        [field: SerializeReference]
        private List<IWeaponController> WeaponControllers { get; set; } = new List<IWeaponController>();
        
        [field: SerializeField] private List<WeaponComponent> TestComponents { get; set; } = new List<WeaponComponent>();

        [field: SerializeField]
        private UnityEvent<HashSet<IAbility>, IEnumerable<GameObject>> OnAbilitiesReleased { get; set; } =
            new UnityEvent<HashSet<IAbility>, IEnumerable<GameObject>>();

        private Dictionary<Type, IWeaponController> WeaponControllersByType { get; } =
            new Dictionary<Type, IWeaponController>();
        
        public int CurrentComboIndex { get; private set; }
        public float AttackDuration { get; set; }
        public int ComponentCapacity { get; private set; } = 3;

        private Timer ComboResetTimer { get; set; }

        private void Awake() {
            foreach (IWeaponController controller in this.WeaponControllers) {
                this.WeaponControllersByType[controller.GetType()] = controller;
            }
        }

        private void Start() {
            this.Stats.Initialise();
            foreach (IWeaponController controller in this.WeaponControllers) {
                controller.Possess(this, this.Stats);
                if (controller is WeaponComponentController componentController) {
                    int i = 0;
                    foreach (WeaponComponent component in this.TestComponents) {
                        componentController.AddComponent(component, i);
                        i += 1;
                    }
                }
            }
            // this.WeaponControllers.ForEach(controller => controller.Possess(this, this.Stats));
        }

        public bool HasController<C>(out IWeaponController controller) where C : IWeaponController {
            return this.WeaponControllersByType.TryGetValue(typeof(C), out controller);
        }

        public bool HasController<C>(out C controller) where C : IWeaponController {
            if (this.HasController<C>(out IWeaponController c)) {
                controller = (C)c;
                return true;
            }
            
            controller = default;
            return false;
        }

        public void ReleaseAbilities(HashSet<IAbility> abilities, IEnumerable<GameObject> carriers) {
            this.OnAbilitiesReleased.Invoke(abilities, carriers);
        }

        public void NextCombo(int comboLength) {
            this.CurrentComboIndex += 1;
            this.CurrentComboIndex %= comboLength;
        }

        public void ResetComboAfter(float time) {
            this.ComboResetTimer = new Timer(time);
            this.ComboResetTimer.OnTimerFinished += this.ResetCombo;
            this.ComboResetTimer.Start();
        }

        private void ResetCombo() {
            this.CurrentComboIndex = 0;
        }

        public void Attack(
            GameObject instigator, List<string> attackableTags, LayerMask attackableLayers, Vector3 attackPoint,
            Vector3 forward
        ) {
            AttackAction action = new AttackAction(instigator, attackableTags, attackableLayers, attackPoint, forward);
            foreach (IWeaponController controller in this.WeaponControllers) {
                controller.UpdateOnAttack(ref action);
            }
        }

        public void EndAttack() {
            foreach (IWeaponController controller in this.WeaponControllers) {
                controller.UpdatePostAttack();
            }
        }
    }
}
