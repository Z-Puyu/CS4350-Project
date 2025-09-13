using System.Collections.Generic;
using Map.Objectives;
using UnityEngine;
using SaintsField;

namespace Map
{
    [CreateAssetMenu(fileName = "Map Unlock Requirement SO", menuName = "Map related/Map Unlock Requirement SO", order = 1)]
    public sealed class MapUnlockRequirementSO : ScriptableObject

    {
        public Sprite key;
        public List<KillEnemiesObjective> killEnemiesObjectives;
        public List<CollectItemObjective> collectItemObjectives;

        public void AddRequirements(List<KillEnemiesObjective> allkillEnemiesObjectives, List<CollectItemObjective> allCollectItemObjectives)
        {
            foreach (var objective in killEnemiesObjectives)
            {
                allkillEnemiesObjectives.Add(objective);
            }
            foreach (var objective in collectItemObjectives)
            {
                allCollectItemObjectives.Add(objective);
            }
        }
    }
}