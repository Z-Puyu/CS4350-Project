using System;
using System.Collections.Generic;
using DataStructuresForUnity.Runtime.Utilities;
using GameplayAbilities.Runtime.Attributes;
using Projectiles.Runtime;
using SaintsField;
using UnityEngine;
using WeaponsSystem.Runtime.Attacks;
using WeaponsSystem.Runtime.WeaponComponents;

namespace WeaponsSystem.Runtime.Weapons
{
    [DisallowMultipleComponent]
    public class Weapon : MonoBehaviour
    {
        [field: SerializeField, Required] private AttributeSet Stats { get; set; }

        [field: SerializeReference]
        private List<IWeaponController> WeaponControllers { get; set; } = new List<IWeaponController>();

        [field: SerializeField] public List<WeaponComponent> TestComponents { get; set; } = new List<WeaponComponent>();
        [field: SerializeField] private List<AudioClip> attackSoundEffects { get; set; } = new List<AudioClip>();

        public int CurrentComboIndex { get; private set; }
        public float AttackDuration { get; set; }
        public int ComponentCapacity { get; private set; } = 3;
        private ProjectileShooterMode projectileMode = ProjectileShooterMode.Default;
        public ProjectileShooterMode PreviousProjectileMode { get; private set; }

        public ProjectileShooterMode ProjectileMode
        {
            get => this.projectileMode;
            set
            {
                this.PreviousProjectileMode = this.projectileMode;
                this.projectileMode = value;
            }
        }

        private Timer ComboResetTimer { get; set; }

        private void Start()
        {
            this.Stats.Initialise();
            foreach (IWeaponController controller in this.WeaponControllers)
            {
                controller.Possess(this, this.Stats);
                if (controller is WeaponComponentController componentController)
                {
                    int i = 0;
                    foreach (WeaponComponent component in this.TestComponents)
                    {
                        componentController.AddComponent(component, i);
                        i += 1;
                    }
                }
            }
            // this.WeaponControllers.ForEach(controller => controller.Possess(this, this.Stats));
        }

        public void NextCombo(int comboLength)
        {
            this.CurrentComboIndex += 1;
            this.CurrentComboIndex %= comboLength;
        }

        public void ResetComboAfter(float time)
        {
            this.ComboResetTimer = new Timer(time);
            this.ComboResetTimer.OnTimerFinished += this.ResetCombo;
            this.ComboResetTimer.Start();
        }

        private void ResetCombo()
        {
            this.CurrentComboIndex = 0;
        }

        public void Attack(
            GameObject instigator, List<string> attackableTags, LayerMask attackableLayers, Vector3 attackPoint,
            Vector3 forward
        )
        {
            AttackAction action = new AttackAction(instigator, attackableTags, attackableLayers, attackPoint, forward);
            foreach (IWeaponController controller in this.WeaponControllers)
            {
                controller.UpdateOnAttack(ref action);
            }
        }

        public void EndAttack()
        {
            foreach (IWeaponController controller in this.WeaponControllers)
            {
                controller.UpdatePostAttack();
            }
        }
        public void AddComponent(WeaponComponent newComponent)
        {
            // Remove all existing components — only 1 active at a time
            TestComponents.Clear();

            // Add the new one
            TestComponents.Add(newComponent);
            foreach (IWeaponController controller in this.WeaponControllers)
            {
                controller.Possess(this, this.Stats);
                if (controller is WeaponComponentController componentController)
                {
                    const int singleSlotIndex = 0;
                    // 1) Attempt to remove any existing component at index 0.
                    // This ensures the old component's effects are disabled (if RemoveComponent is successful).
                    bool removedOld = componentController.RemoveComponent(singleSlotIndex);

                    if (removedOld)
                    {
                        UnityEngine.Debug.Log($"[EQUIPMENT] Cleared previous component from slot {singleSlotIndex}.");
                    }

                    // 2) Add the new component to the single slot (index 0).
                    // Since TestComponents only has one element (the new one), we use index 0 for the single-slot system.
                    componentController.AddComponent(newComponent, singleSlotIndex);

                    UnityEngine.Debug.Log($"[EQUIPMENT] Equipped {newComponent.name} into slot {singleSlotIndex}.");
                }
            }
        }

    }
}
