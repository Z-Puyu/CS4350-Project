using System.Collections.Generic;
using DataStructuresForUnity.Runtime.Utilities;
using GameplayAbilities.Runtime.Attributes;
using GameplayEffects.Runtime;
using SaintsField;
using UnityEngine;
using WeaponsSystem.Weapons;

namespace WeaponsSystem.WeaponComponents {
    [CreateAssetMenu(fileName = "New Weapon Component", menuName = "Weapons/Weapon Component")]
    public class WeaponComponent : ScriptableObject {
        [field: SerializeReference] private IEffect<IDataReader<string, int>, AttributeSet> EffectOnWeapon { get; set; }

        [field: SerializeReference, SaintsDictionary]
        private SaintsDictionary<int, IEffect<IDataReader<string, int>, AttributeSet>> EffectsOnAttack { get; set; } =
            new SaintsDictionary<int, IEffect<IDataReader<string, int>, AttributeSet>>();
        
        private IRunnableEffect EffectOnWeaponInstance { get; set; }
        private List<IRunnableEffect> EffectsOnAttackInstance { get; set; } = new List<IRunnableEffect>();
        
        public void Enable(Weapon weapon, AttributeSet stats) {
            this.EffectOnWeaponInstance = this.EffectOnWeapon.Apply(stats, stats);
            this.EffectOnWeaponInstance.Start();
        }

        public void Disable(Weapon weapon, AttributeSet stats) {
            this.EffectOnWeaponInstance.Cancel();
            this.EffectOnWeaponInstance = null;
        }

        public void PreprocessAttack(Weapon weapon, AttributeSet stats) {
            int index = weapon.CurrentComboIndex;
            if (!this.EffectsOnAttack.TryGetValue(index, out IEffect<IDataReader<string, int>, AttributeSet> effect)) {
                return;
            }

            IRunnableEffect effectInstance = effect.Apply(stats, stats);
            this.EffectsOnAttackInstance.Add(effectInstance);
            effectInstance.Start();
        }

        public void PostprocessAttack(Weapon weapon, AttributeSet stats) {
            this.EffectsOnAttackInstance.ForEach(effect => effect.Cancel());
            this.EffectsOnAttackInstance.Clear();
        }
    }
}
