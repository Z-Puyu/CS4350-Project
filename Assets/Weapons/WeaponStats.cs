using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.Modifiers;
using GameplayEffects.Runtime;
using SaintsField;
using UnityEngine;
using WeaponsSystem;
using WeaponsSystem.Projectiles;

namespace Weapons {
    [DisallowMultipleComponent]
    public abstract class WeaponStats : MonoBehaviour, IDataReader<string, int> {
        [field: SerializeReference]
        private List<IAttackController> WeaponControllers { get; set; } = new List<IAttackController>();
        
        private List<IRunnableEffect> CurrentAttackEffects { get; } = new List<IRunnableEffect>();
        
        public virtual void ActivateAttackData(int comboIndex = 0) {
            foreach (IAttackController controller in this.WeaponControllers) {
                controller.Configure(this, comboIndex);
            }
        }

        public void ActivateEffectForCurrentAttack(IRunnableEffect effect) {
            effect.Start();
            this.CurrentAttackEffects.Add(effect);
        }

        internal void DeactivateEffectsForCurrentAttack() {
            this.CurrentAttackEffects.ForEach(effect => effect.Cancel());
            this.CurrentAttackEffects.Clear();
        }
        
        public abstract void Set(string key, int value);
        
        public abstract bool HasValue(string key, out int value);

        IDataReader<string, int> IDataReader<string, int>.With(string key, int value) {
            this.Set(key, value);
            return this;
        }
    }
}
