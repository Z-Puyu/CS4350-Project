using System.Collections.Generic;
using DataStructuresForUnity.Runtime.Utilities;
using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.Attributes;
using GameplayEffects.Runtime;
using Projectiles.Runtime;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using WeaponsSystem.Runtime.Attacks;
using WeaponsSystem.Runtime.Weapons;

namespace WeaponsSystem.Runtime.WeaponComponents {
    [CreateAssetMenu(fileName = "New Weapon Component", menuName = "Weapons/Components/Weapon Component")]
    public class WeaponComponent : ScriptableObject {
        private enum ProjectileConfig { None, OnWeapon, OnAttack }
        
        [field: SerializeField] private ProjectileConfig ChangeProjectileMode { get; set; } = ProjectileConfig.None;

        [field: SerializeField, SaintsDictionary, ShowIf(nameof(this.ChangeProjectileMode), ProjectileConfig.OnAttack)]
        private SaintsDictionary<int, ProjectileShooterMode> ProjectileModesOnAttack { get; set; } =
            new SaintsDictionary<int, ProjectileShooterMode>();
        
        [field: SerializeField, ShowIf(nameof(this.ChangeProjectileMode), ProjectileConfig.OnWeapon)] 
        private ProjectileShooterMode ProjectileModeOnWeapon { get; set; } = ProjectileShooterMode.Single;
        
        [field: SerializeReference] private IEffect<IDataReader<string, int>, AttributeSet> EffectOnWeapon { get; set; }

        [field: SerializeReference]
        private List<IEffect<IDataReader<string, int>, AttributeSet>> EffectsOnAttack { get; set; } =
            new List<IEffect<IDataReader<string, int>, AttributeSet>>();

        [field: SerializeField, SaintsDictionary]
        private SaintsDictionary<int, Ability> AttachedAbilities { get; set; } = new SaintsDictionary<int, Ability>();
        
        private IRunnableEffect EffectOnWeaponInstance { get; set; }
        private List<IRunnableEffect> EffectsOnAttackInstance { get; set; } = new List<IRunnableEffect>();
        
        public void Enable(Weapon weapon, AttributeSet stats) {
            if (this.EffectOnWeapon == null) {
                return;
            }
            
            this.EffectOnWeaponInstance = this.EffectOnWeapon.Apply(stats, stats);
            this.EffectOnWeaponInstance.Start();
        }

        public void Disable(Weapon weapon, AttributeSet stats) {
            if (this.EffectOnWeaponInstance == null) {
                return;           
            }
            
            this.EffectOnWeaponInstance.Cancel();
            this.EffectOnWeaponInstance = null;
        }

        private void ToggleProjectileMode(Weapon weapon, int index) {
            switch (this.ChangeProjectileMode) {
                case ProjectileConfig.OnWeapon
                        when weapon.HasController(out WeaponProjectileAttackController controller):
                    controller.ProjectileMode = this.ProjectileModeOnWeapon;
                    break;
                case ProjectileConfig.OnAttack
                        when weapon.HasController(out WeaponProjectileAttackController controller) &&
                             this.ProjectileModesOnAttack.TryGetValue(index, out ProjectileShooterMode mode):
                    controller.ProjectileMode = mode;
                    break;
            }
        }

        private void AttachAbilities(Weapon weapon, int index) {
            if (this.AttachedAbilities.TryGetValue(index, out Ability ability) &&
                weapon.HasController(out WeaponAttackController controller)) {
                controller.Attach(ability);
            }
        }

        public void PreprocessAttack(Weapon weapon, AttributeSet stats) {
            int index = weapon.CurrentComboIndex;
            this.ToggleProjectileMode(weapon, index);
            this.AttachAbilities(weapon, index);
            if (this.EffectsOnAttack[index] is null) {
                return;
            }
            
            IRunnableEffect effectInstance = this.EffectsOnAttack[index].Apply(stats, stats);
            this.EffectsOnAttackInstance.Add(effectInstance);
            effectInstance.Start();
        }

        public void PostprocessAttack(Weapon weapon, AttributeSet stats) {
            this.EffectsOnAttackInstance.ForEach(effect => effect.Cancel());
            this.EffectsOnAttackInstance.Clear();
        }
    }
}
