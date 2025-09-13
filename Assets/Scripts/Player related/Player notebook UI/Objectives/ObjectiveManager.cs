using Map.Objectives;
using UnityEngine;
using System.Collections.Generic;
using ModularItemsAndInventory.Runtime.Items;
using ModularItemsAndInventory.Runtime.Inventory;
using Map.Objectives;
using Map;

namespace Objectives
{
    public class ObjectiveManager : MonoBehaviour
    {
        [SerializeField] private List<KillEnemiesObjective> allKillEnemiesObjectives = new List<KillEnemiesObjective>();
        [SerializeField] private List<CollectItemObjective> allCollectItesmObjectives = new List<CollectItemObjective>();
        [SerializeField] private List<MapUnlockRequirementSO> allRequirements;
        public void AddEnemyToObjective()
        {
            foreach (var objective in new List<KillEnemiesObjective>(allKillEnemiesObjectives))
            {
                objective.AddProgress();
            }
        }

        public void AddItemToObjective(Component component, object item)
        {
            ItemData mappedItem;
            ItemKey itemToAdd = (ItemKey)((object[])item)[0];
            ItemDatabase.TryGet(itemToAdd, out mappedItem);
            foreach (var objective in new List<CollectItemObjective>(allCollectItesmObjectives))
            {
                objective.AddProgress(mappedItem, allCollectItesmObjectives);
            }
        }
        
        public void AddObjectives(Component component, object requirementsSo)
        {
            MapUnlockRequirementSO mapUnlockRequirementSo = (MapUnlockRequirementSO) ((object[]) requirementsSo)[0];
            allRequirements.Add(mapUnlockRequirementSo);
            mapUnlockRequirementSo.AddRequirements(allKillEnemiesObjectives, allCollectItesmObjectives);
        }
    }   
}
