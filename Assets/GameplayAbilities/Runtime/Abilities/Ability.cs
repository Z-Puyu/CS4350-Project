using System;
using System.Collections.Generic;
using System.Linq;
using DataStructuresForUnity.Runtime.GeneralUtils;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameplayAbilities.Runtime.Abilities {
    [CreateAssetMenu(fileName = "New Ability", menuName = "Gameplay Abilities/Ability", order = 0)]
    public class Ability : ScriptableObject, IAbility {
        [field: SerializeField] internal bool IsObtainable { get; private set; } = true;
        [field: SerializeField] internal string Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField, MinValue(0)] private int Cooldown { get; set; }

        [field: SerializeField] 
        public List<SpawnableAbilityObject> SpawnableEffectsOnTarget { get; private set; } =
            new List<SpawnableAbilityObject>();
        
        [field: SerializeField] 
        public List<SpawnableAbilityObject> SpawnableEffectsOnSelf { get; private set; } =
            new List<SpawnableAbilityObject>();
        
        [field: SerializeReference, ReferencePicker] 
        public List<GameplayEffectData> EffectsOnTarget { get; private set; } = new List<GameplayEffectData>();
        
        [field: SerializeReference, ReferencePicker] 
        public List<GameplayEffectData> EffectsOnSelf { get; private set; } = new List<GameplayEffectData>();

        private int Duration {
            get {
                if (this.EffectsOnTarget.Any(effect => effect.ActualDuration < 0) ||
                    this.EffectsOnSelf.Any(effect => effect.ActualDuration < 0)) {
                    return -1;
                }

                int durationOnTarget = 0;
                if (this.EffectsOnTarget.Count > 0) {
                    durationOnTarget = this.EffectsOnTarget.Max(effect => effect.ActualDuration);
                }

                int durationOnSelf = 0;
                if (this.EffectsOnSelf.Count > 0) {
                    durationOnSelf = this.EffectsOnSelf.Max(effect => effect.ActualDuration);
                }
                
                return Math.Max(durationOnTarget, durationOnSelf);
            }
        }

        private int CooldownTime => this.Cooldown + Math.Max(0, this.Duration);
        public AbilityInfo Info => new AbilityInfo(this.Id, this.CooldownTime, this.Duration);

        public bool IsUsable(AttributeSet instigator, AttributeSet target) {
            return true;
        }
        
        public void Invoke(AbilitySystem instigator, AbilitySystem target, GameplayEffectExecutionArgs args = null) {
            foreach (SpawnableAbilityObject spawn in this.SpawnableEffectsOnTarget) {
                ObjectSpawner.Pull(spawn.PoolableId, spawn, target.transform);
            }
            
            if (this.EffectsOnTarget.Count == 0) {
                return;
            }
            
            args ??= instigator.CreateEffectExecutionArgs().Build();
            instigator.Inflict(target, this, this.EffectsOnTarget.Select(data => data.Instantiate(args)));
        }

        public void Activate(AbilitySystem instigator, Vector3 position) {
            foreach (SpawnableAbilityObject spawn in this.SpawnableEffectsOnSelf) {
                ObjectSpawner.Pull(spawn.PoolableId, spawn, position, Quaternion.identity, instigator.transform)
                             .Activate(new AbilityData(this.Info, instigator.AttributeSet, position));
            }
            
            if (this.EffectsOnSelf.Count == 0) {
                return;
            }
            
            GameplayEffectExecutionArgs args = instigator.CreateEffectExecutionArgs().Build();
            foreach (GameplayEffectData data in this.EffectsOnSelf) {
                GameplayEffect effect = data.Instantiate(args);
                instigator.AddEffect(effect, this);
            }
        }
    }
}

