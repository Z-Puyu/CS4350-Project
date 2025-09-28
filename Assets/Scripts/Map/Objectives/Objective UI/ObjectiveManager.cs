using UnityEngine;
using System.Collections.Generic;
using Events;
using Game.Enemies;
using ModularItemsAndInventory.Runtime.Items;
using ModularItemsAndInventory.Runtime.Inventory;
using SaintsField;

namespace Map.Objectives.Objective_UI
{
    public class ObjectiveManager : MonoBehaviour
    {
        [SerializeField] private MapUnlockRequirementSO chosenRequirement;
        [SerializeField] private ObjectiveUIManager objectiveUIManager;
        [SerializeField] private List<KillEnemiesObjective> allKillEnemiesObjectives = new List<KillEnemiesObjective>();
        [SerializeField] private List<CollectItemObjective> allCollectItesmObjectives = new List<CollectItemObjective>();
        [SerializeField] private List<MapUnlockRequirementSO> allRequirements;
        [SerializeField] private SaintsDictionary<Objective, MapUnlockRequirementSO> objectiveToRequirement;
        [SerializeField] private SaintsDictionary<MapUnlockRequirementSO, int> taskCounterForRequirement;
        [SerializeField] private CrossObjectEventWithDataSO broacastNumberOfIncompleteObjectives;
        
        public void OpenNotebook()
        {
            objectiveUIManager.OpenUI();
            foreach (var mapRequirement in allRequirements)
            {
                objectiveUIManager.SpawnPanel(mapRequirement, chosenRequirement == mapRequirement);
            }   
        }

        public void SetChosenUnlockedRequirementSO(Component component, object urSO)
        {
            this.chosenRequirement = (MapUnlockRequirementSO)((object[])urSO)[0];
            UpdateIncompleteObjectivesText();
        }
        
        public void AddEnemyToObjective(Component component, object enemy)
        {
            EnemyData enemyToKill = (EnemyData)((object[])enemy)[0];
             foreach (var objective in new List<KillEnemiesObjective>(allKillEnemiesObjectives))
            {
                objective.AddProgress(
                    this,
                    allKillEnemiesObjectives,
                    enemyToKill
                    );
            }
        }

        public void AddItemToObjective(Component component, object item)
        {
            ItemData mappedItem;
            ItemKey itemToAdd = (ItemKey)((object[])item)[0];
            ItemDatabase.TryGet(itemToAdd, out mappedItem);
            foreach (var objective in new List<CollectItemObjective>(allCollectItesmObjectives))
            {
                objective.AddProgress(
                    this,
                    mappedItem, 
                    allCollectItesmObjectives
                    );
            }
        }

        public void HandleDeletion(Objective objective)
        {
            MapUnlockRequirementSO mapRequirementSO = objectiveToRequirement[objective];
            objectiveToRequirement.Remove(objective);
            taskCounterForRequirement[mapRequirementSO] -= 1;
            if (taskCounterForRequirement[mapRequirementSO] == 0)
            {
                taskCounterForRequirement.Remove(mapRequirementSO);
                allRequirements.Remove(mapRequirementSO);
                if (chosenRequirement == mapRequirementSO)
                {
                    if (allRequirements.Count > 0)
                    {
                        chosenRequirement = allRequirements[0];
                        UpdateIncompleteObjectivesText();
                    }
                }
             }
        }
        
        public void AddObjectives(Component component, object requirementsSo)
        {
            MapUnlockRequirementSO mapUnlockRequirementSo = (MapUnlockRequirementSO) ((object[]) requirementsSo)[0];
            allRequirements.Add(mapUnlockRequirementSo);
            mapUnlockRequirementSo.AddRequirements(
                taskCounterForRequirement,
                objectiveToRequirement,
                allKillEnemiesObjectives, 
                allCollectItesmObjectives
                );
            if (chosenRequirement == null)
            {
                chosenRequirement = mapUnlockRequirementSo;
                UpdateIncompleteObjectivesText();
            }
        }

        private void UpdateIncompleteObjectivesText()
        {
            broacastNumberOfIncompleteObjectives.TriggerEvent(this, chosenRequirement.GetNumberOfIncompleteObjectives());
        }
    }   
}
