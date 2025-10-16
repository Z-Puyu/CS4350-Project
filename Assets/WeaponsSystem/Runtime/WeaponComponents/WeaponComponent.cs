using System.Collections.Generic;
using DataStructuresForUnity.Runtime.Utilities;
using GameplayAbilities.Runtime.Attributes;
using GameplayEffects.Runtime;
using ModularItemsAndInventory.Runtime.Items;
using Projectiles.Runtime;
using SaintsField;
using UnityEngine;
using WeaponsSystem.Runtime.Weapons;

namespace WeaponsSystem.Runtime.WeaponComponents {
    [CreateAssetMenu(fileName = "New Weapon Component", menuName = "Weapons/Components/Weapon Component")]
    public class WeaponComponent : ScriptableObject {
        [SerializeField] public string Id;
        [Header("Shop Info")]
        [SerializeField] public string itemName;
        [SerializeField] public string Description;

        [SerializeField] public Sprite icon;
        [SerializeField] public int price;
        [SerializeField] public List<MaterialCost> craftingMaterials = new List<MaterialCost>();
        [SerializeField] public string rarity = "Common";
        [SerializeField] public int levelRequirement = 1;
        [SerializeField] public string category { get; private set; }

        [field: SerializeField] List<ProjectileShooterMode> ProjectileModesOnAttack { get; set; } = new List<ProjectileShooterMode>();
        [field: SerializeField] ProjectileShooterMode ProjectileModeOnWeapon { get; set; } = ProjectileShooterMode.Default;
        [field: SerializeReference] private IEffect<IDataReader<string, int>, AttributeSet> EffectOnWeapon { get; set; }

        [field: SerializeReference]
        private List<IEffect<IDataReader<string, int>, AttributeSet>> EffectsOnAttack { get; set; } =
            new List<IEffect<IDataReader<string, int>, AttributeSet>>();
        
        private IRunnableEffect EffectOnWeaponInstance { get; set; }
        private List<IRunnableEffect> EffectsOnAttackInstance { get; set; } = new List<IRunnableEffect>();

        public void Enable(Weapon weapon, AttributeSet stats)
        {
            if (this.EffectOnWeapon == null)
            {
                return;
            }

            this.EffectOnWeaponInstance = this.EffectOnWeapon.Apply(stats, stats);
            this.EffectOnWeaponInstance.Start();
        }
        
        [System.Serializable]
        public struct MaterialCost
        {
            public ItemData material;
            public int amount;
        }

        public void Disable(Weapon weapon, AttributeSet stats) {
            if (this.EffectOnWeaponInstance == null) {
                return;           
            }
            
            this.EffectOnWeaponInstance.Cancel();
            this.EffectOnWeaponInstance = null;
        }

        public void PreprocessAttack(Weapon weapon, AttributeSet stats) {
            int index = weapon.CurrentComboIndex;
            if (this.ProjectileModeOnWeapon != ProjectileShooterMode.None) {
                weapon.ProjectileMode = this.ProjectileModeOnWeapon;
            } else if (index >= 0 && index < this.ProjectileModesOnAttack.Count) {
                weapon.ProjectileMode = this.ProjectileModesOnAttack[index];
            }
            
            if (index < 0 || index >= this.EffectsOnAttack.Count || this.EffectsOnAttack[index] == null) {
                return;
            }
            
            IRunnableEffect effectInstance = this.EffectsOnAttack[index].Apply(stats, stats);
            this.EffectsOnAttackInstance.Add(effectInstance);
            effectInstance.Start();
        }

        public void PostprocessAttack(Weapon weapon, AttributeSet stats) {
            weapon.ProjectileMode = ProjectileShooterMode.Default;
            this.EffectsOnAttackInstance.ForEach(effect => effect.Cancel());
            this.EffectsOnAttackInstance.Clear();
        }
    }
}
