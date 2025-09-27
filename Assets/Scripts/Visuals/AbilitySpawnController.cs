using System;
using System.Collections.Generic;
using Common;
using DataStructuresForUnity.Runtime.GeneralUtils;
using GameplayAbilities.Runtime.Abilities;
using SaintsField;
using UnityEngine;

namespace Visuals {
    [DisallowMultipleComponent, RequireComponent(typeof(BoundingRect))]
    public sealed class AbilitySpawnController : MonoBehaviour {
        private Dictionary<string, List<SpawnableAbilityObject>> SpawnedEffects { get; set; } =
            new Dictionary<string, List<SpawnableAbilityObject>>();
        
        public void ShowAbility(AbilityData data) {
            string id = data.Info.Id;
            Ability ability = PerkDatabase.GetAbility(id);
            foreach (KeyValuePair<SpawnableAbilityObject, int> effect in ability.SpawnableEffects) {
                for (int i = 0; i < effect.Value; i += 1) {
                    if (!effect.Key) {
                        continue;
                    }

                    SpawnableAbilityObject spawn = ObjectSpawner.Pull(
                        effect.Key.PoolableId, effect.Key, this.transform
                    );
                        
                    spawn.Activate(data.Info);
                    if (this.SpawnedEffects.TryGetValue(id, out List<SpawnableAbilityObject> list)) {
                        list.Add(spawn);
                    } else {
                        this.SpawnedEffects.Add(id, new List<SpawnableAbilityObject> { spawn });
                    }
                }
            }
        }
    }
}
