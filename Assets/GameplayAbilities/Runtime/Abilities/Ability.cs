using System.Collections.Generic;
using System.Linq;
using DataStructuresForUnity.Runtime.GeneralUtils;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities {
    [CreateAssetMenu(fileName = "New Ability", menuName = "Gameplay Abilities/Ability", order = 0)]
    public class Ability : ScriptableObject, IAbility {
        [field: SerializeField] internal string Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField, MinValue(0)] private int Cooldown { get; set; }
        
        [field: SerializeField, SaintsDictionary("Effect", "Multiplicity")] 
        internal SaintsDictionary<PoolableObject, int> SpawnableEffects { get; private set; } = 
            new SaintsDictionary<PoolableObject, int>();
        
        [field: SerializeReference, ReferencePicker] 
        public List<GameplayEffectData> Effects { get; private set; } = new List<GameplayEffectData>();
        
        public AbilityInfo Info => new AbilityInfo(this.Cooldown, this.Effects.Count);

        public void StartAbility(Vector3 at) {
            foreach (KeyValuePair<PoolableObject, int> effect in this.SpawnableEffects) {
                for (int i = 0; i < effect.Value; i += 1) {
                    ObjectSpawner.Pull(effect.Key.PoolableId, effect.Key, at, Quaternion.identity);
                }
            }
        }

        public IEnumerable<GameplayEffect> GenerateEffects(GameplayEffectExecutionArgs args) {
            return this.Effects.Select(effect => new GameplayEffect(effect, args));
        }

        public bool IsUsable(AttributeSet instigator, AttributeSet target) {
            return true;
        }
    }
}

