using System;
using DataStructuresForUnity.Runtime.GeneralUtils;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.Projectiles {
    [Serializable]
    public class ProjectileEffectData {
        [field: SerializeField] public ProjectileEffectType Type { get; private set; }
        
        [field: SerializeField, RequireType(typeof(ParticleSystem))] 
        public PoolableObject ParticleAsset { get; private set; }
    }
}
