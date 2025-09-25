using System;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.Projectiles {
    [Serializable]
    public class ProjectileEffectData {
        [field: TypeReference(superTypes: new[] { typeof(IProjectileEffect) })]
        private TypeReference Type { get; set; }
        
        [field: SerializeReference, ReferencePicker] 
        private GameplayEffectData Effect { get; set; }

        public GameplayEffect Instantiate(AttributeSet weapon, GameplayEffectExecutionArgs args) {
            return this.Effect.Instantiate(weapon, args);
        }
    }
}
