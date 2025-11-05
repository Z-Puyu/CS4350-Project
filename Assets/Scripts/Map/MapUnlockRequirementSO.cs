using System.Collections.Generic;
using Game.Enemies;
using Map.Objectives;
using Map.Objectives.Objective_UI;
using Player_related.Player_things_to_note_ui_manager;
using UnityEngine;
using SaintsField;
using TMPro;

namespace Map
{
    [CreateAssetMenu(fileName = "Map Unlock Requirement SO", menuName = "Map related/Map Unlock Requirement SO", order = 1)]
    public sealed class MapUnlockRequirementSO : ScriptableObject

    {
        public string title;
        public Sprite key;
        public List<KillEnemiesObjective> killEnemiesObjectives;
        public List<CollectItemObjective> collectItemObjectives;
        public Boss bossToSpawn;

        public void SetTitle(TextMeshProUGUI objectiveText)
        {
            objectiveText.text = title;
        }

        public int GetNumberOfIncompleteObjectives()
        {
            int count = 0;
            foreach (var objective in killEnemiesObjectives)
            {
                count += (!objective.IsComplete() ? 1 : 0);
            }
            foreach (var objective in collectItemObjectives)
            {
                count += (!objective.IsComplete() ? 1 : 0);
            }
            return count;
        }

        public void SpawnTextWhenObjectiveIsCleared(PlayerThingsToNoteUIManager playerThingsToNoteUIManager)
        {
            playerThingsToNoteUIManager.SpawnText("Cleared all objective requirements for " + title);
        }
        
        public void SpawnObjectiveText(GameObject objectiveTextPrefab, Transform content, List<GameObject> spawnedObjectiveText)
        {
            foreach (var objective in killEnemiesObjectives)
            {
                GameObject objectiveTextObj = Instantiate(objectiveTextPrefab, content);
                spawnedObjectiveText.Add(objectiveTextObj);
                objective.SetText(objectiveTextObj.GetComponent<ObjectiveText>());
            }
            foreach (var objective in collectItemObjectives)
            {
                GameObject objectiveTextObj = Instantiate(objectiveTextPrefab, content);
                spawnedObjectiveText.Add(objectiveTextObj);
                objective.SetText(objectiveTextObj.GetComponent<ObjectiveText>()); 
            }
        }

        public void AddRequirements(SaintsDictionary<MapUnlockRequirementSO, int> taskCounterForRequirement, SaintsDictionary<Objective, MapUnlockRequirementSO> objectiveToRequirement, List<KillEnemiesObjective> allkillEnemiesObjectives, List<CollectItemObjective> allCollectItemObjectives)
        {
            foreach (var objective in killEnemiesObjectives)
            {
                allkillEnemiesObjectives.Add(objective);
                if (!taskCounterForRequirement.Contains(this))
                {
                    taskCounterForRequirement[this] = 0;
                }
                taskCounterForRequirement[this] += 1;
                objectiveToRequirement[objective] = this;
            }
            foreach (var objective in collectItemObjectives)
            {
                allCollectItemObjectives.Add(objective);
                if (!taskCounterForRequirement.Contains(this))
                {
                    taskCounterForRequirement[this] = 0;
                }
                taskCounterForRequirement[this] += 1;
                objectiveToRequirement[objective] = this;
            }
        }
    }
}