using System.Collections.Generic;
using DataStructuresForUnity.Runtime.GeneralUtils;
using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem {
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/WeaponData", order = 0)]
    public class WeaponData : ScriptableObject {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField, MinValue(0)] public float ComboResetTime { get; private set; } = 3;
        [field: SerializeField] public AttributeTable WeaponAttributes { get; private set; }
        [field: SerializeField] public List<Ability> EffectsOnHit { get; private set; } = new List<Ability>();
        
        [field: SerializeField, RequireType(typeof(PoolableObject))] 
        public PoolableObject ParticleEffectOnHit { get; private set; }
    }
}
