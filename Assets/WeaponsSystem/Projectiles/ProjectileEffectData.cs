using System;
using System.Collections.Generic;
using System.Linq;
using DataStructuresForUnity.Runtime.GeneralUtils;
using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.Projectiles {
    [Serializable]
    public class ProjectileEffectData {
        [field: SerializeField, TypeReference(superTypes: new[] { typeof(IProjectileEffect) })]
        public TypeReference Type { get; private set; }
        
        [field: SerializeField, RequireType(typeof(ParticleSystem))] 
        public PoolableObject ParticleAsset { get; private set; }
        
        [field: SerializeField] public List<Ability> EffectsOnTarget { get; private set; } = new List<Ability>();
        [field: SerializeField] public List<Ability> EffectsOnInstigator { get; private set; } = new List<Ability>();
    }
}
